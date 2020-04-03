using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;


namespace OnceUponATime_1
{
    [TestFixture]
    public class StoryTest
    {
        private MainHero GetMH()
        {
            var hero = new MainHero("MainHero");
            hero.SetSympathies( new Dictionary<string, int> {{"Tom", 2},{"Olivia", 4}, {"Jake", -5}});
            return hero;
        }

        [Test]
        public void TestMainHeroSetSympGeneral()
        {
            var hero = GetMH();
            hero.Name.Should().Be("MainHero");
            hero.MainLover.Should().Be("Olivia");
            hero.Sympathies["Olivia"].Should().Be(4);
            hero.Sympathies["Tom"].Should().Be(2);
            hero.Sympathies["Jake"].Should().Be(-5);
        }

        [Test]
        public void TestMainHeroSeSympMainLoverOliviaBoth()
        {
            var hero = GetMH();
            var d = new Dictionary<string, int>{{"MainLover", 2},{"Olivia", 4}};
            hero.SetSympathies(d);
            hero.Sympathies["Olivia"].Should().Be(6);
            hero.Sympathies["Tom"].Should().Be(2);
            hero.Sympathies["Jake"].Should().Be(-5);
        }

        [Test]
        public void TestMainHeroSetSympOnlyMainLover()
        {
            var hero = GetMH();
            var d = new Dictionary<string, int>{{"MainLover", 2},{"Tom", 4}};
            hero.SetSympathies(d);
            hero.Sympathies["Olivia"].Should().Be(6);
            hero.Sympathies["Tom"].Should().Be(6);
            hero.Sympathies["Jake"].Should().Be(-5);
        }
        
        [Test]
        public void TestJsonParserGetStories()
        {
            var jp = new JsonParser<Story>(); 
            jp.SetFilename(@"\StoriesConfig.json");
            var list = jp.Get();
            list.Count.Should().Be(1);
            var hero = GetMH();
            var story = new Story("Test", new List<int> {1}, hero);
            list[0].Should().BeEquivalentTo(story);
            jp.Dispose();
        }

        [Test]
        public void TestStoryGetNextSeries()
        {
            var story = new Story("Test", new List<int> {1}, GetMH());
            GameLogic.StoryName = story.Name;
            story.GetNextSeries().Should().Be(@"\series\Test\1_1.json");
        }
        
        [Test]
        public void TestJsonParserGetIScenes()
        {
            var jp = new JsonParser<Scene>();
            jp.SetFilename(@"\series\Test\1_1.json");
            var list = jp.Get();
            list.Count.Should().Be(4);
            jp.Dispose();
        }

        [Test]
        public void TestJsonParserSet()
        {
            var jp = new JsonParser<Story>();
            var story = new Story("Test", new List<int> {1}, GetMH());
            story.SetSeries(0, 0);
            jp.Set("StoriesConfig.json", new List<Story> {story});
            jp.Dispose();
        }

        [Test]
        public void TestJsonParserGetPlayer()
        {
            var jp = new JsonParser<Player>();
            jp.SetFilename(@"\GameConfig.json");
            var p = jp.GetOneT();
            p.Diamonds.Should().Be(15);
            p.Keys.Should().Be(5);
            p.TotalDays.Should().Be(0);
            p.LastVisit.Should().Be(DateTime.Today);

            p.CheckIfFirstVisit().Should().Be(true);
            p.TotalDays.Should().Be(1);
            p.LastVisit.Should().Be(DateTime.Today);

        }

        [Test]
        public void TestPlayerDateTimeSubstract()
        {
            var p = new Player(15, 10, 12, DateTime.Today);
            p.CheckIfFirstVisit().Should().Be(false);
            p.LastVisit.Should().Be(DateTime.Today);
            p.TotalDays.Should().Be(12);
            
            p = new Player(0, 0, 12, DateTime.Parse("02/04/2020"));
            p.CheckIfFirstVisit().Should().Be(true);
            p.LastVisit.Should().Be(DateTime.Today);
            p.TotalDays.Should().Be(13);
            
            
            p = new Player(0, 0, 12, DateTime.Parse("01/04/2020"));
            p.CheckIfFirstVisit().Should().Be(true);
            p.LastVisit.Should().Be(DateTime.Today);
            p.TotalDays.Should().Be(1);

        }
        
    }
}