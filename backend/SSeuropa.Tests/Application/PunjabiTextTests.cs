using FluentAssertions;
using SSeuropa.Application.Services;

namespace SSeuropa.Tests.Application;

/// <summary>Roman Punjabi is shown as people type it (same rules as content/plainPunjabi.mjs).</summary>
public class PunjabiTextTests
{
    [Theory]
    [InlineData("Maiṁ Simran hāṁ.", "Main Simran haan.")]
    [InlineData("Tūṁ / Tusīṁ", "Tu / Tusi")]
    [InlineData("Uh (muṇḍā / ādmī)", "Uh (munda / aadmi)")]
    [InlineData("Uh (kuṛī)", "Uh (kudi)")]
    [InlineData("Āp jī", "Aap ji")]
    [InlineData("Sat Srī Akāl! Maiṁ Giulia hāṁ. Tuhāḍā nām kī hai?", "Sat Sri Akal! Main Giulia haan. Tuhada naam ki hai?")]
    [InlineData("Mainūṁ vī! Aman, tusīṁ kitthoṁ de ho?", "Mainu vi! Aman, tusi kitthon de ho?")]
    [InlineData("ṭhīk · dūr · pāṇī · sāīz", "theek · door · paani · saiz")]
    [InlineData("città · perché · più", "città · perché · più")] // Italian is never touched
    [InlineData("Main Simran haan.", "Main Simran haan.")]
    public void Plain_WritesPunjabiAsPeopleTypeIt(string input, string expected) =>
        PunjabiText.Plain(input).Should().Be(expected);

    [Fact]
    public void TableMeanings_ReadsSeparateAndCombinedColumns()
    {
        List<List<string>> separate = [["Italiano", "English", "Punjabi"], ["io", "I", "Main"]];
        PunjabiText.TableMeanings(separate).Should().Equal(("I", "Main"));

        List<List<string>> combined = [["Forma", "English · Punjabi"], ["questo", "this coat · eh kot"], ["x", "hottest"]];
        PunjabiText.TableMeanings(combined).Should().Equal(("this coat", "eh kot"), ("hottest", null));

        List<List<string>> plain = [["Persona", "essere"], ["io", "sono"]];
        PunjabiText.TableMeanings(plain).Should().BeEmpty();
    }
}
