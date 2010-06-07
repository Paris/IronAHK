using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeTernaryOperatorExpression : CodeExpression
    {
        public CodeTernaryOperatorExpression() { }

        public CodeTernaryOperatorExpression(CodeExpression condition, CodeExpression trueBranch, CodeExpression falseBranch)
        {
            Condition = condition;
            TrueBranch = trueBranch;
            FalseBranch = falseBranch;
        }

        public CodeExpression Condition { get; set; }

        public CodeExpression TrueBranch { get; set; }

        public CodeExpression FalseBranch { get; set; }
    }
}
