#include <fbxsdk.h>
#include <cstdint>

using namespace fbxsdk;

/*
 * Generate some numbers.
 *
 * Then we run a C# program (in FbxVector4Test) that reads in the numbers and
 * verifies that it gets the same ones.
 *
 * The tests use numbers that lead to exact results in double-precision
 * arithmetic to make sure we don't get worked up about rounding.
 *
 * Some of the tests use numbers that lead to inexact results in
 * single-precision, on purpose.
 */
void printItem(double x, char sep = '\n') {
    union {
        double d;
        uint64_t u;
    };
    d = x;
    printf("%g/0x%llx%c", d, u, sep);
}

void printItem(FbxVector4 x, char sep = '\n') {
    printItem(x[0], ',');
    printItem(x[1], ',');
    printItem(x[2], ',');
    printItem(x[3], sep);
}

void printItem(FbxVector2 x, char sep = '\n') {
    printItem(x[0], ',');
    printItem(x[1], sep);
}

#define printResult(op) { printf("%s:%s:", testName, #op); printItem((op)); }

template <class Vector>
void RunCommands(const char *testName, Vector a, Vector b)
{
    printResult(a);
    printResult(b);

    printResult(-a);

    printResult(a + 2);
    printResult(a - 2);
    printResult(a * 2);
    printResult(a / 2);

    printResult(a + b);
    printResult(a - b);
    printResult(a * b);
    printResult(a / b);

    printResult(a.DotProduct(b));
    printResult(a.Length());
    printResult(a.SquareLength());
    printResult(a.Distance(b));
}

int main()
{
    // Pick a vector with values that don't fit in a float, to make sure
    // we're actually using doubles all the way through.
    // A float can only handle 24 bits. A double can handle 53,
    // so we can get an exact dot product for up to 26 bits.
    FbxVector4 a ((1<<26) + 1, (1<<26) + 2, (1<<26) + 3, 0.25);

    // Pick a vector that we can add, sub, mul, div componentwise with the
    // prior vector without error.
    FbxVector4 b (5, 6, 7);

    // Run all the vector4 commands we can.
    const char *testName = "FbxVector4";
    RunCommands(testName, a, b);
    printResult(a.CrossProduct(b)); // not available for vector2

    // Same thing but for vector 2.
    testName = "FbxVector2";
    RunCommands(testName, FbxVector2((1<<26) + 1, (1<<26) + 2), FbxVector2(5, 6));

    return 0;
}
