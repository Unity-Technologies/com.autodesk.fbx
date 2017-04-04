#include <cassert>

/* The handle class for weak pointers. */
#include <unordered_map>

struct WeakPointerHandle;
typedef std::unordered_map<void*, WeakPointerHandle*> HandleMap;
static HandleMap g_handles;

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
        if(m_numRefs == 1) {
            if (m_ptr != nullptr) {
                HandleMap::iterator it = g_handles.find(m_ptr);
                if (it != g_handles.end()) {
                    g_handles.erase(it);
                }
            }
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
            HandleMap::iterator it = g_handles.find(p);
            if (it != g_handles.end()) {
                it->second->m_ptr = nullptr;
                g_handles.erase(it);
            }
        }

        public:
        static void *AllocateMemory(size_t n) {
            return ::malloc(n);
        }

        static void *AllocateZero(size_t n, size_t sz) {
            return ::calloc(n, sz);
        }

        static void *Reallocate(void *old, size_t n) {
            void *newPtr = ::realloc(old, n);
            if (old && old != newPtr) { MarkFree(old); }
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
    size_t m_numRefs;
};
