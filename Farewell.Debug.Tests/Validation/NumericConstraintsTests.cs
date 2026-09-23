using Farewell.Abstractions.Validation;
using Farewell.Validation.Constraints;

namespace Farewell.Debug.Tests.Validation;

public sealed class NumericConstraintsTests
{
    [Theory]
    [InlineData(0, 0, 150, true)]
    [InlineData(150, 0, 150, true)]
    [InlineData(75, 0, 150, true)]
    [InlineData(-1, 0, 150, false)]
    [InlineData(151, 0, 150, false)]
    public void OutOfRange_Int_Check(int value, int min, int max, bool expected)
    {
        var constraint = new NumericConstraints.OutOfRangeConstraint<int>(min, max);
        Assert.Equal(expected, constraint.Check(value));
    }

    [Theory]
    [InlineData(0.0, 0.0, 1.0, true)]
    [InlineData(1.0, 0.0, 1.0, true)]
    [InlineData(0.5, 0.0, 1.0, true)]
    [InlineData(-0.1, 0.0, 1.0, false)]
    [InlineData(1.1, 0.0, 1.0, false)]
    public void OutOfRange_Double_Check(double value, double min, double max, bool expected)
    {
        var constraint = new NumericConstraints.OutOfRangeConstraint<double>(min, max);
        Assert.Equal(expected, constraint.Check(value));
    }

    [Fact]
    public void OutOfRange_Code_IsOutOfRange()
    {
        var constraint = new NumericConstraints.OutOfRangeConstraint<int>(0, 100);
        Assert.Equal(ValidationCode.OutOfRange, constraint.Code);
    }
}