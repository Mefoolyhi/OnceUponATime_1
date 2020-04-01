using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;


namespace OnceUponATime_1
{
    [TestFixture]
    public class StoryTest
    {
        private MainHero GetMH()
        {
            var hero = new MainHero("Ann");
            hero.SetSympathies( new Dictionary<string, int> {{"Tom", 2},{"Olivia", 4}, {"Jake", -5}});
            return hero;
        }

        [Test]
        public void TestMainHeroSetSymp()
        {
            var hero = GetMH();
            hero.Name.Should().Be("Ann");
            hero.MainLover.Should().Be("Olivia");
            hero.Sympathies["Olivia"].Should().Be(4);
            hero.Sympathies["Tom"].Should().Be(2);
            hero.Sympathies["Jake"].Should().Be(-5);
        }
        
        [Test]
        public void TestJsonParserGetStories()
        {
            var jp = new JsonParser(); 
            jp.SetFilename("StoriesConfig.json");
            var list = jp.GetStories();
            list.Count.Should().Be(1);
            var hero = GetMH();
            var story = new Story("Test", new List<int> {1}, hero);
            story.SetSeries(1, 1);
            list[0].Should().BeEquivalentTo(story);
            
        }
    }
}