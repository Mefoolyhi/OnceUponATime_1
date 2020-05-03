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
            using var jp = new JsonParser<List<Story>>();
            jp.SetFilenameForReading(@"\StoriesConfigTest.json");
            var list = jp.GetObject();
            list.Count.Should().Be(1);
            var hero = GetMH();
            var story = new Story("Test", new List<int> {1}, hero);
            list[0].Should().BeEquivalentTo(story);
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
            using var jp = new JsonParser<List<Scene>>();
            jp.SetFilenameForReading(@"\series\Test\1_1.json");
            var list = jp.GetObject();
            list.Count.Should().Be(4);
            object[] scenes = {
                new Scene(new List<Phrase> {new Phrase("Jake", "Hi!"),
                    new Phrase("Tom", "Hello!"),
                    new Phrase("", "Bonsoir!")},
                    "General", "images/Test/background.png",null, null),
                new Scene(new List<Phrase> {new Phrase("Jake", "Hi!"),
                        new Phrase("MainHero", "Hello!")},
                    "Logic", "images/Test/background.png",null, null),
                new Scene(new List<Phrase> {new Phrase("MainHero", "Hi!"),
                        new Phrase("Tom", "Would you like to go swimming?")},
                    "Intuitional", "images/Test/background.png",null, null),
                new Scene(null, null, "images/Test/background.png","MainHero", 
                    new List<Choice> { new Choice("Yes", 0, 2, 15,
                        new Dictionary<string, int> {{"Olivia", -1}, {"Tom", 3}}, 
                        new List<Scene>{new Scene(new List<Phrase> {new Phrase("Tom", "Hi!"),
                                new Phrase("MainHero", "Hello!")},
                            "Love", "images/Test/background.png",null, null)}),
                        new Choice("No", 2, 0, 0, 
                            new Dictionary<string, int> {{"MainLover", 3}, {"Olivia", -1}, {"Tom", -1}},
                            new List<Scene>{ new Scene(new List<Phrase> {new Phrase("MainLover", "Hi!"),
                                    new Phrase("MainHero", "Hello!")},
                                "Love", "images/Test/background.png",null, null)})
                    })};
            list.Should().BeEquivalentTo(scenes);
        }

        [Test]
        public void TestJsonParserSave()
        {
            using var jp = new JsonParser<List<Story>>();
            jp.SetFilenameForWriting(@"\1.json");
            var story = new Story("Test", new List<int> {1}, GetMH());
            story.SetSeries(0, 0);
            jp.SaveToFile(new List<Story> {story});
            jp.SetFilenameForReading(@"\1.json");
            var list = jp.GetObject();
            list.Count.Should().Be(1);
            list[0].Should().BeEquivalentTo(story);
            
            
            using var pp = new JsonParser<Player>();
            pp.SetFilenameForWriting(@"\2.json");
            var player = new Player(20, 2, 2, DateTime.Today);
            pp.SaveToFile(player);
            pp.SetFilenameForReading(@"\2.json");
            var p = pp.GetObject();
            p.Diamonds.Should().Be(player.Diamonds);
            p.Keys.Should().Be(player.Keys);
            p.TotalDays.Should().Be(player.TotalDays);
            p.LastVisit.Should().Be(DateTime.Today);
            p.TryUpdateLastVisitAndDaysCountRecords().Should().Be(false);
            p.TotalDays.Should().Be(player.TotalDays);
            p.LastVisit.Should().Be(DateTime.Today);
        }

        [Test]
        public void TestJsonParserGetPlayer()
        {
            using var jp = new JsonParser<Player>();
            jp.SetFilenameForReading(@"\GameConfigTest.json");
            var p = jp.GetObject();
            p.Diamonds.Should().Be(15);
            p.Keys.Should().Be(5);
            p.TotalDays.Should().Be(0);
            p.LastVisit.Should().Be(DateTime.Today);
            p.TryUpdateLastVisitAndDaysCountRecords().Should().Be(true);
            p.TotalDays.Should().Be(1);
            p.LastVisit.Should().Be(DateTime.Today);
        }

        [Test]
        public void TestPlayerDateTimeSubstract()
        {
            var p = new Player(15, 10, 12, DateTime.Today);
            p.TryUpdateLastVisitAndDaysCountRecords().Should().Be(false);
            p.LastVisit.Should().Be(DateTime.Today);
            p.TotalDays.Should().Be(12);
            
            p = new Player(0, 0, 12, DateTime.Parse("21/04/2020")); //yesterday
            p.TryUpdateLastVisitAndDaysCountRecords().Should().Be(true);
            p.LastVisit.Should().Be(DateTime.Today);
            p.TotalDays.Should().Be(13);
            
            
            p = new Player(0, 0, 12, DateTime.Parse("01/04/2020"));
            p.TryUpdateLastVisitAndDaysCountRecords().Should().Be(true);
            p.LastVisit.Should().Be(DateTime.Today);
            p.TotalDays.Should().Be(1);
        }
        
    }
}