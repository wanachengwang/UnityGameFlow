using ILRuntime.CLR.Method;

namespace ILHotFix {
    public class ILHFInvoker {
        public static string TypeFullName { get { return typeof(ILHFInvoker).FullName; } }
        public static string TypeName { get { return typeof(ILHFInvoker).Name; } }
        public static string ParamFieldName { get { return "Params"; } }
        public static string InvokeFuncName { get { return "Invoke"; } }
        public static ILRuntime.Runtime.Enviorment.AppDomain ILRTDomain;
        public object[] Params;
        private IMethod _method;

        public ILHFInvoker(IMethod method) {
            _method = method;
            Params = new object[method.Parameters.Count];
        }
        public object Invoke() {
            if (ILRTDomain != null) {
                return ILRTDomain.Invoke(_method, null, Params);
            }
            return null;
        }
    }
}