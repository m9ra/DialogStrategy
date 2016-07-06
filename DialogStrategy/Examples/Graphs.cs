using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Examples
{
    class Graphs
    {
        public static readonly string IsPlayable = "is playable";
        public static readonly string HasDirector = "has director";
        public static readonly string HasActor = "has actor";
        public static readonly string IsDirector = "is director of";
        public static readonly string IsActor = "is actor of";
        public static readonly string Satisfy = "satisfy";
        public static readonly string Reigns = "reigns";

        public static readonly string Thing = "thing";
        public static readonly string Film = "film";
        public static readonly string Person = "person";
        public static readonly string Actor = "actor";
        public static readonly string Director = "director";
        public static readonly string President = "president";
        public static readonly string State = "state";

        public static readonly string UserGoal = "user goal";
        public static readonly string Play = "play";
        public static readonly string PlayVideo = "play video";
        public static readonly string See = "see";
        public static readonly string Who = "who";

        public static readonly string WoodyAllen = "Woody Allen";
        public static readonly string Manhattan = "Manhattan";
        public static readonly string AnnieHall = "Annie Hall";
        public static readonly string BarackObama = "Barack Obama";
        public static readonly string USA = "usa";

        public static Graph CreateGraph()
        {
            var graph = new Graph();
            graph.Add(Film, Graph.IsRelation, Thing);
            graph.Add(UserGoal, Graph.IsRelation, Thing);
            graph.Add(Person, Graph.IsRelation, Thing);
            graph.Add(Graph.Flag, Graph.IsRelation, Thing);
            graph.Add(Graph.Active, Graph.IsRelation, Graph.Flag);

            videoDomain(graph);
            presidentDomain(graph);

            return graph;
        }

        public static void FilmWithWoody(Graph graph)
        {
            //I want to see a Woody Allen Film.
            graph.Inform(See, Graph.Active);
            graph.Inform(Film, Graph.Active);
            graph.Inform(WoodyAllen, Graph.Active);
        }

        public static void WhoIsPresidentInUSA(Graph graph)
        {
            graph.Inform(Who, Graph.Active);
            graph.Inform(President, Graph.Active);
            graph.Inform(USA, Graph.Active);
        }

        private static void presidentDomain(Graph graph)
        {
            graph
                .Is(President, Person)
                .Is(BarackObama, President)

                .Is(USA, State)
                .Add(BarackObama, Reigns, USA);
                ;

        }

        private static void videoDomain(Graph graph)
        {
            graph.Add(Actor, Graph.IsRelation, Person);
            graph.Add(Director, Graph.IsRelation, Person);

            graph.Add(Play, Graph.IsRelation, UserGoal);
            graph.Add(PlayVideo, Graph.IsRelation, Play);

            graph.Add(WoodyAllen, Graph.IsRelation, Actor);
            graph.Add(WoodyAllen, IsActor, Manhattan);
            graph.Add(WoodyAllen, IsDirector, Manhattan);
            graph.Add(WoodyAllen, IsActor, AnnieHall);
            graph.Add(WoodyAllen, IsDirector, AnnieHall);

            graph.Add(Manhattan, Graph.IsRelation, Film);
            graph.Add(AnnieHall, Graph.IsRelation, Film);

            //special nodes
            graph.Add(PlayVideo, Satisfy, See);
        }

    }
}
