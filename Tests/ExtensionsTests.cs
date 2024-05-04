using Toca3DifficultyEditor;

namespace Tests;

public class ExtensionsTests {

    [Theory]
    [InlineData("B", new byte[] { 0x42 })]
    [InlineData("ÿ", new byte[] { 0xC3, 0xBF })]
    [InlineData("\u2603", new byte[] { 0xE2, 0x98, 0x83 })]
    [InlineData("\ud83d\udca9", new byte[] { 0xF0, 0x9F, 0x92, 0xA9 })]
    public void stringToBytes(string input, byte[] expected) {
        input.ToBytes().Should().Equal(expected);
    }

}