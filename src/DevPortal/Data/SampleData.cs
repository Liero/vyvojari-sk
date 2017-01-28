using DevPortal.Web.Models.BlogViewModels;
using DevPortal.Web.Models.ForumViewModels;
using DevPortal.Web.Models.NewsViewModels;
using DevPortal.Web.Models.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Data
{
    public class SampleData
    {
        private static SampleData _instance;
        public static SampleData Instance => _instance ?? (_instance = new SampleData());

        public static string[] Titles =
        {
            "Visual Studio 2017 RC",
            "Je dostupný TypeScript 2.1 RC",
            "Konferencia ShowIT 2017 - dovolenka pre ajťákov!",
            "Surface Studio a iné",
            "Nová verzia esri js api 3.18",
        };

        public static string[] UserNames = {
            "liero", "T", "harisson314", "siro", "vlko"
        };

        public static string[] LoremIpsum =
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
            "But I must explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you a complete account of the system, and expound the actual teachings of the great explorer of the truth, the master-builder of human happiness. No one rejects, dislikes, or avoids pleasure itself, because it is pleasure, but because those who do not know how to pursue pleasure rationally encounter consequences that are extremely painful. Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but because occasionally circumstances occur in which toil and pain can procure him some great pleasure. To take a trivial example, which of us ever undertakes laborious physical exercise, except to obtain some advantage from it? But who has any right to find fault with a man who chooses to enjoy a pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant pleasure?"
        };

        public static string[] UserPictures =
        {
            "~/media/userpicture.jpg",
            "http://vyvojari.sk/Avatar.ashx?UserName=T",
            "http://vyvojari.sk/Avatar.ashx?UserName=Siro",
            "http://vyvojari.sk/Avatar.ashx?UserName=Liero"
        };

        public static string[] BlogTitles =
        {
            "ExJS TypeScript emitter",
            "Video straming v ASP.NET Core",
            "Sencha ExtJS - Čo je nové",
            "Monády",
            "Generovanie PDF pomocou iTextSharp a XmlWorker v MVC"
        };

        public static string[] BlogExternalLinks =
        {
            "http://blog.vyvojari.sk/tomas/archive/2016/12/17/exjs-typescript-emitter.aspx",
            "http://harrison314.github.io/CoreStreaming.html",
            "http://blog.vyvojari.sk/tomas/archive/2016/12/17/sencha-extjs-o-je-nov-233.aspx",
            "http://harrison314.github.io/Monads.html",
            "http://blog.vyvojari.sk/xxxmatko/archive/2016/12/06/generovanie-pdf-pomocou-itextsharp-a-xmlworker-v-mvc.aspx"
        };

        public SampleData()
        {
            var rnd = new Random();

            News = Enumerable.Range(0, 20).Select(i => new NewsItemViewModel
            {
                Title = from(Titles, i),
                UserName = from(UserNames, i),
                Content = from(LoremIpsum, i),
                Comments = rnd.Next(3) > 0 ? new List<CommentViewModel>() :
                   GenerateComments().Skip(i).Take(rnd.Next(10)).ToList()
            }).ToArray();

            ForumPosts = Enumerable.Range(3, 20).Select(i => new ForumPostViewModel
            {
                Title = from(Titles, i),
                UserName = from(UserNames, i),
                Content = from(LoremIpsum, i),
                Created = DateTime.Now.AddHours(-i * 1.9),
                Avatar = from(UserPictures, i)
            }).ToArray();

            BlogPosts = Enumerable.Range(15, 10).Select(i => new BlogInfoViewModel
            {
                Title = from(BlogTitles, i),
                UserName = from(UserNames, i),
                Description = from(LoremIpsum, i),
                ExternalLink = from(BlogExternalLinks,i),
                Created = DateTime.Now.AddHours(-i * 1.9),
                Avatar = from(UserPictures, i),
                CommentsCount = (i*3+1) % 11
            }).ToArray();
        }

        public NewsItemViewModel[] News { get; private set; }

        public ForumPostViewModel[] ForumPosts { get; private set; }

        public BlogInfoViewModel[] BlogPosts { get; private set; }

        public static IEnumerable<CommentViewModel> GenerateComments()
        {
            return Enumerable.Range(0, int.MaxValue).Select(i => new CommentViewModel
            {
                UserName = from(UserNames, i),
                Message = from(LoremIpsum, i),
            });
        }

        private static T from<T>(T[] source, int index) => source[index % source.Length];
    }
}
