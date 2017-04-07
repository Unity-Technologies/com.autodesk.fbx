#include <cassert>

/* The handle class for weak pointers. */
#include <unordered_map>

struct WeakPointerHandle;
typedef std::unordered_map<void*, WeakPointerHandle*> HandleMap;
static HandleMap g_handles;

#ifdef MEMORY_DEBUG
#include <unordered_set>
static std::unordered_set<void*> AllocatedBlocks;
#endif

struct WeakPointerHandle {
    static inline WeakPointerHandle *GetHandle(void *p) {
        if (!p) {
            return nullptr;
        }
        HandleMap::iterator it = g_handles.find(p);
        if (it != g_handles.end()) {
            it->second->m_numRefs++;
            return it->second;
        } else {
            WeakPointerHandle * weak = new WeakPointerHandle(p);
            g_handles.insert(std::make_pair(p, weak));
            return weak;
        }
    }

    template <typename T>
        static inline bool DerefHandle(void *vHandle, T **out_pointer) {
            WeakPointerHandle *handle = static_cast<WeakPointerHandle*>(vHandle);
            if (!handle) {
                // null handle => just a null value
                *out_pointer = nullptr;
                return true;
            }
            if (handle->m_ptr) {
                // handle is valid; use it
                *out_pointer = static_cast<T*>(handle->m_ptr);
                return true;
            }
            // handle is invalid; return false to throw an exception
            *out_pointer = nullptr;
            return false;
        }

    void ReleaseReference() {
#ifdef MEMORY_DEBUG
        if (m_ptr != 0) {
          fprintf(stderr, "Releasing %llx (%d refs)\n", uint64_t(m_ptr), m_numRefs);
          assert(AllocatedBlocks.count(m_ptr) != 0);
        }
#endif
        if(m_numRefs == 1) {
            if (m_ptr != nullptr) {
                HandleMap::iterator it = g_handles.find(m_ptr);
                if (it != g_handles.end()) {
                    g_handles.erase(it);
                }
            }
            m_numRefs = -1;
            delete this;
        } else {
            assert(m_numRefs > 1);
            m_numRefs--;
        }
    }

    class Allocators {
        private:
        // Mark the pointer as being freed.
        // If there's a weak pointer handle for p, we'll set its pointer to null,
        // and we'll remove it from the handles. The C# users will be responsible for
        // releasing their references to it.
        static inline void MarkFree(void *p) {
            if (!p) { return; }
            #ifdef MEMORY_DEBUG
            if (AllocatedBlocks.find(p) == AllocatedBlocks.end()) {
                fprintf(stderr, "Duplicate free at %llx\n", uint64_t(p));
                assert(AllocatedBlocks.count(p) != 0);
            }
            AllocatedBlocks.erase(p);
            #endif
            HandleMap::iterator it = g_handles.find(p);
            if (it != g_handles.end()) {
                it->second->m_ptr = nullptr;
                g_handles.erase(it);
            }
        }
        static inline void MarkAllocated(void *p) {
            #ifdef MEMORY_DEBUG
            if (!AllocatedBlocks.insert(p).second) {
                fprintf(stderr, "Duplicate allocation at %llx\n", uint64_t(p));
                assert(AllocatedBlocks.count(p) == 0);
            }
            #endif
        }

        public:
        static void *AllocateMemory(size_t n) {
            void *p = ::malloc(n);
            MarkAllocated(p);
            return p;
        }

        static void *AllocateZero(size_t n, size_t sz) {
            void *p = ::calloc(n, sz);
            MarkAllocated(p);
            return p;
        }

        static void *Reallocate(void *old, size_t n) {
            void *newPtr = ::realloc(old, n);
            if (old != newPtr) {
                MarkFree(old);
                MarkAllocated(newPtr);
            }
            return newPtr;
        }

        static void FreeMemory(void *old) {
            ::free(old);
            MarkFree(old);
        }
    };

    private:
    inline WeakPointerHandle(void *p) : m_ptr(p), m_numRefs(1) { }
    inline ~WeakPointerHandle() { }

    void *m_ptr;
    int m_numRefs;
};
