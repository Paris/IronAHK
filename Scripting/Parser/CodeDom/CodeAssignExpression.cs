using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeAssignExpression : CodeExpression
    {
        CodeExpression left, right;

        public CodeAssignExpression() { }

        public CodeAssignExpression(CodeExpression left, CodeExpression right)
        {
            this.left = left;
            this.right = right;
        }

        public CodeExpression Left
        {
            get { return left; }
            set { left = value; }
        }

        public CodeExpression Right
        {
            get { return right; }
            set { right = value; }
        }

        static public implicit operator CodeAssignStatement(CodeAssignExpression assign)
        {
            return new CodeAssignStatement(assign.Left, assign.Right);
        }
    }
}
