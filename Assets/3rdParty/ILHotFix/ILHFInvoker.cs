using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ILHFInvoker {
	public static ILRuntime.Runtime.Enviorment.AppDomain ILRTDomain;

	public ILHFInvoker(IMethod method) {
		_method = method;
		Parameters = new object[method.Parameters.Count];
	}

	private IMethod _method;
	public object[] Parameters;
	public object Invoke() {
		if (ILRTDomain != null)	{
			return ILRTDomain.Invoke(_method, null, Parameters);
		}
		return null;
	}
}