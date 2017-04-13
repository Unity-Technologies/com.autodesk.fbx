// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
//
// This script depends on the Mono.Reflection.MethodBodyReader by the JB Evain,
// see https://github.com/jbevain/mono.reflection (it's under an MIT license).
// ***********************************************************************
#if ENABLE_COVERAGE_TEST

using System.Collections.Generic;
using System.Reflection;

static class CoverageTester
{
	static Dictionary<MethodInfo, HashSet<MethodInfo>> m_calls = new Dictionary<MethodInfo, HashSet<MethodInfo>>();
    static Dictionary<MethodInfo, HashSet<MethodInfo>> m_reflectionCalls = new Dictionary<MethodInfo, HashSet<MethodInfo>>();

    /// <summary>
    /// Dig through the instructions for 'method' and see which methods it
    /// calls, and what methods those methods call in turn recursively.
    ///
    /// This is cached, so the first call for any given function will be
    /// expensive but subsequent ones will be cheap.
    /// </summary>
    public static HashSet<MethodInfo> GetCalls(MethodInfo method)
    {
        if(m_calls.ContainsKey(method)) {
            return m_calls[method];
        }

        // Look at the current method and in DFS, find all the methods it calls.
        var stack = new List<MethodInfo>();
        var visited = new HashSet<MethodInfo>();
        stack.Add(method);
        while(stack.Count > 0) {
            var top = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            if (visited.Contains(top)) {
                continue;
            }
            visited.Add(top);

            // If we have already seen this method, we can copy all its calls in and stop 
            // our search here.
            if (m_calls.ContainsKey(top)) {
                visited.UnionWith(m_calls[top]);
                continue;
            }

            List<Mono.Reflection.Instruction> instructions;
            try {
                instructions = Mono.Reflection.MethodBodyReader.GetInstructions(top);
            } catch (System.ArgumentException xcp) {
                // ignore the method having no body
                continue;
            }
            foreach (var instruction in instructions) {
                // look at the instructions of this method; see if they are a function call.
                // if so, recursively look into that function
                var calledMethod = instruction.Operand as MethodInfo;
                if (calledMethod == null) { continue; }

                // It's a function call, so recursively look into it.
                // TODO: we should recursively call GetCalls on each method
                // we call here, but in a way that doesn't hit infinite loops.
                // Then we'd hit in cache more often.
                stack.Add(calledMethod);
            }
        }

        // Every function we visited is a function that this function calls!
        m_calls[method] = visited;
        return visited;
	}

    /// <summary>
    /// Clear the cache; useful if you just registered a new reflection call.
    /// </summary>
	public static void ClearCache()
    {
		m_calls = new Dictionary<MethodInfo, HashSet<MethodInfo>>();
    }

    /// <summary>
    /// If you're using reflection to call methods, they won't be picked
    /// up automatically. This lets you add that information.
    ///
    /// You might want to clear the cache afterwards.
    /// </summary>
    public static void RegisterReflectionCall(MethodInfo from, MethodInfo to)
    {
        HashSet<MethodInfo> calls;
		if (m_reflectionCalls.TryGetValue(from, out calls)) {
			calls.Add(to);
		} else {
			HashSet<MethodInfo> tos = new HashSet<MethodInfo>();
			tos.Add(to);
			m_reflectionCalls[from] = tos;
		}
    }

    public static bool TestCoverage(IEnumerable<MethodInfo> MethodsToCover,
        IEnumerable<MethodInfo> RootMethods,
		out HashSet<MethodInfo> out_HitMethods,
		out HashSet<MethodInfo> out_MissedMethods
	)
    {
        var calledMethods = new HashSet<MethodInfo>();
        foreach(var myMethod in RootMethods) {
            calledMethods.UnionWith(GetCalls(myMethod));
        }

		out_MissedMethods = new HashSet<MethodInfo>();
		out_HitMethods = new HashSet<MethodInfo>();
		foreach(var proxyMethod in MethodsToCover) {
            if (!calledMethods.Contains(proxyMethod)) {
				out_MissedMethods.Add(proxyMethod);
            } else {
				out_HitMethods.Add(proxyMethod);
            }
        }
		if (out_MissedMethods.Count == 0) {
			return true;
		} else {
			return false;
		}
	}

	private static string GetMethodSignature(MethodInfo info)
	{
		var builder = new System.Text.StringBuilder();
		builder.Append(info.ReturnType.Name);
		builder.Append(' ');
		builder.Append(info.DeclaringType.Name);
		builder.Append('.');
		builder.Append(info.Name);
		builder.Append('(');
		var args = info.GetParameters();
		if (args.Length > 0) {
			builder.Append(args[0].ParameterType.Name);
		}
		for(var i = 1; i < args.Length; ++i) {
			builder.Append(", ");
			builder.Append(args[i].ParameterType.Name);
		}
		builder.Append(')');
		return builder.ToString();
	}

	public static string MakeCoverageMessage(		
		HashSet<MethodInfo> HitMethods,
		HashSet<MethodInfo> MissedMethods
		)

	{
		var missed = new List<string>();
		var hit = new List<string>();
		foreach(var h in HitMethods) { hit.Add(GetMethodSignature(h)); }
		foreach(var m in MissedMethods) { missed.Add(GetMethodSignature(m)); }
		missed.Sort();
		hit.Sort();
		return string.Format("Failed to call:\n\t{0}\nSucceeded to call:\n\t{1}",
			string.Join("\n\t", missed.ToArray()),
			string.Join("\n\t", hit.ToArray()));
    }
}
#endif
