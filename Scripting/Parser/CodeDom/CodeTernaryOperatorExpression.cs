using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeTernaryOperatorExpression : CodeExpression
    {
        CodeExpression condition, trueBranch, falseBranch;

        public CodeTernaryOperatorExpression() { }

        public CodeTernaryOperatorExpression(CodeExpression condition, CodeExpression trueBranch, CodeExpression falseBranch)
        {
            this.condition = condition;
            this.trueBranch = trueBranch;
            this.falseBranch = falseBranch;
        }

        public CodeExpression Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        public CodeExpression TrueBranch
        {
            get { return trueBranch; }
            set { trueBranch = value; }
        }

        public CodeExpression FalseBranch
        {
            get { return falseBranch; }
            set { falseBranch = value; }
        }
    }
}
