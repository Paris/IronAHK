using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        // Build a cache of points where we need to look for exception trinkets
        // An exception trinket is an object denoting the offsets of try, catch and finally blocks
        void MineExTrinkets(MethodBody Body, List<int> ExceptionTrinkets)
        {
            foreach(ExceptionHandlingClause Clause in Body.ExceptionHandlingClauses)
            {
                // Only handle catch and finally. TODO: fault and filter
                if(Clause.Flags != ExceptionHandlingClauseOptions.Clause &&
                   Clause.Flags != ExceptionHandlingClauseOptions.Finally) 
                    continue;
                
                ExceptionTrinkets.Add(Clause.TryOffset);
                ExceptionTrinkets.Add(Clause.HandlerOffset);
                ExceptionTrinkets.Add(Clause.HandlerOffset+Clause.HandlerLength);
            }
        }
        
        void CopyTryCatch(ILGenerator Gen, int i, MethodBody Body, List<int> ExceptionTrinkets)
        {
            // Quick check to see if we want to walk through the list
            if(!ExceptionTrinkets.Contains(i)) return;
            
            foreach(ExceptionHandlingClause Clause in Body.ExceptionHandlingClauses)
            {
                if(Clause.Flags != ExceptionHandlingClauseOptions.Clause &&
                   Clause.Flags != ExceptionHandlingClauseOptions.Finally) 
                    continue;
                
                // Look for an ending of an exception block first!
                if(Clause.HandlerOffset+Clause.HandlerLength == i)
                    Gen.EndExceptionBlock();
                
                // If this marks the beginning of a try block, emit that
                if(Clause.TryOffset == i)
                    Gen.BeginExceptionBlock();
                
                // Also check for the beginning of a catch block
                if(Clause.HandlerOffset == i && Clause.Flags == ExceptionHandlingClauseOptions.Clause)
                    Gen.BeginCatchBlock(Clause.CatchType);
                
                // Lastly, check for a finally block
                if(Clause.HandlerOffset == i && Clause.Flags == ExceptionHandlingClauseOptions.Finally)
                    Gen.BeginFinallyBlock();
            }
        }
    }
}

