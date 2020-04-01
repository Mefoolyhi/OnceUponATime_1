using System.IO;
using FluentAssertions;
using NUnit.Framework;


namespace OnceUponATime_1
{
    [TestFixture]
    public class StoryTest
    {
        [Test]
        public void JsonParserSeriesTest()
        {
            var jp = new JsonParser(
                @"C:\Users\Елена\RiderProjects\OnceUponATime_1\OnceUponATime_1"); //TODO Вставьте свою корневую директорию
            jp.SetFilename("series/Test/1.json");
            var series = jp.GetNext();
            series.Story.Should().Be("Test");
            series.Season.Should().Be(1);
            series.SeriesNumber.Should().Be(1);
            series.Filename.Should().Be("series/Test/TestParse.json");
            series.Persons[0].Should().BeEquivalentTo(new Person("Jake", "images/Test/Jake.png"));
            series.Persons[1].Should().BeEquivalentTo(new Person("Tom", "images/Test/Tom.png"));
            series.Persons[2].Should().BeEquivalentTo(new Person("Olivia", "images/Test/Olivia.png"));
        }
    }
}