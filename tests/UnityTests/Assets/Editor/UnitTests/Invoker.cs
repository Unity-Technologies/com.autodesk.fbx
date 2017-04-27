public static class Invoker
{
    /**
     * Invoke a zero-argument instance method.
     */
    public static U Invoke<U>(System.Reflection.MethodInfo method, object instance) {
        try {
            return (U)(method.Invoke(instance, null));
        } catch(System.Reflection.TargetInvocationException xcp) {
            throw xcp.GetBaseException();
        }
    }

    /**
     * Invoke a single-argument instance method.
     */
    public static U Invoke<U>(System.Reflection.MethodInfo method, object instance, object arg) {
        try {
            return (U)(method.Invoke(instance, new object [] { arg }));
        } catch(System.Reflection.TargetInvocationException xcp) {
            throw xcp.GetBaseException();
        }
    }

    /**
     * Invoke a two-argument instance method.
     */
    public static U Invoke<U>(System.Reflection.MethodInfo method, object instance, object arg1, object arg2) {
        try {
            return (U)(method.Invoke(instance, new object [] { arg1, arg2 }));
        } catch(System.Reflection.TargetInvocationException xcp) {
            throw xcp.GetBaseException();
        }
    }

    /**
     * Invoke a single-argument static method.
     */
    public static U InvokeStatic<U>(System.Reflection.MethodInfo method, object arg) {
        try {
            return (U)(method.Invoke(null, new object[] { arg } ));
        } catch(System.Reflection.TargetInvocationException xcp) {
            throw xcp.GetBaseException();
        }
    }

    /**
     * Invoke a two-argument static method.
     */
    public static U InvokeStatic<U>(System.Reflection.MethodInfo method, object arg1, object arg2) {
        try {
            return (U)(method.Invoke(null, new object [] { arg1, arg2 }));
        } catch(System.Reflection.TargetInvocationException xcp) {
            throw xcp.GetBaseException();
        }
    }
}
