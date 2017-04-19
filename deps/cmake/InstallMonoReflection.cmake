# Install the Mono.Reflection package we have installed as a submodule.
#
# It's under MIT license.
install(DIRECTORY ${CMAKE_SOURCE_DIR}/deps/mono.reflection/Mono.Reflection DESTINATION ${CMAKE_BINARY_DIR}/tests/UnityTests/Assets/Plugins)
