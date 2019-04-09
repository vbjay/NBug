using System;

namespace NBug.Tests.Unit.Util
{
    internal static class Mock
    {
        public static void DoWork() => Method4();

        private static void Method1()
        {
            var ex = new DivideByZeroException("Boom!");
            var ex1 = new ArgumentOutOfRangeException("Opps", ex);
            throw new ApplicationException("Failed", ex1);
        }

        private static void Method2() => Method1();
        private static void Method3() => Method2();
        private static void Method4() => Method3();
    }
}