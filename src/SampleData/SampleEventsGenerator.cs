using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;

namespace DevPortal.DesignTimeData
{

    /// <summary>
    /// each time controller an event changes, we whould add it here so we ensure all historical events can be replayed
    /// </summary>
    public class SampleEventsGenerator
    {
        public static string[] UserNames =
        {
            "liero", "T", "harisson314", "siro", "vlko"
        };

        public static string[] Titles =
        {
            "Visual Studio 2017 RC",
            "Je dostupný TypeScript 2.1 RC",
            "Konferencia ShowIT 2017 - dovolenka pre ajťákov!",
            "Surface Studio a iné",
            "Nová verzia esri js api 3.18",
        };

        public static string[] LoremIpsum =
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
            "But I must explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you a complete account of the system, and expound the actual teachings of the great explorer of the truth, the master-builder of human happiness. No one rejects, dislikes, or avoids pleasure itself, because it is pleasure, but because those who do not know how to pursue pleasure rationally encounter consequences that are extremely painful. Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but because occasionally circumstances occur in which toil and pain can procure him some great pleasure. To take a trivial example, which of us ever undertakes laborious physical exercise, except to obtain some advantage from it? But who has any right to find fault with a man who chooses to enjoy a pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant pleasure?"
        };



        public IEnumerable<DomainEvent> News(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int backIndex = count - 1 - i;
                DateTime timestamp = DateTime.Now.AddMinutes(-(backIndex * 100) ^ 2 + 1);

                var created = new NewsItemCreated
                {
                    NewsItemId = Guid.NewGuid(),
                    AuthorUserName = UserNames.Element(i),
                    Title = Titles.Element(i),
                    Content = LoremIpsum.Element(i),
                    Tags = new string[] { "c#", "asp.net-core" },
                    TimeStamp = timestamp
                };
                yield return created;

                if (i > 0)
                {
                    yield return new NewsItemPublished
                    {
                        NewsItemId = created.NewsItemId,
                        TimeStamp = timestamp.AddSeconds(10),
                    };
                }

                if (i == 1)
                {
                    yield return new NewsItemEdited
                    {
                        NewsItemId = created.NewsItemId,
                        Title = created.Title,
                        Content = created.Content + "\n\n**EDIT:** *should be italic*",
                        EditorUserName = UserNames[0],
                        Tags = created.Tags,
                        TimeStamp = timestamp,
                    };
                }

                if (i == 2)
                {
                    for (short j = 0; j < 10; j++)
                    {
                        yield return new NewsItemCommented
                        {
                            UserName = UserNames.Element(i + j),
                            Content = LoremIpsum.Element(i + j),
                            NewsItemId = created.NewsItemId,
                            CommentId = Guid.NewGuid(),
                            TimeStamp = timestamp.AddSeconds(-(10 - j) * 5)
                        };
                    }
                }
            }
        }

        public IEnumerable<DomainEvent> Forum(int count)
        {
            var rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                int backIndex = count - 1 - i;
                DateTime timestamp = DateTime.Now.AddMinutes(-(backIndex * 100) ^ 2 + 1);

                var forumThreadCreated = new ForumThreadCreated
                {
                    ForumThreadId = Guid.NewGuid(),
                    AuthorUserName = UserNames.Element(i),
                    Title = Titles.Element(i),
                    Content = LoremIpsum.Element(i),
                    Tags = new string[] { "c#", "asp.net-core" },
                    TimeStamp = timestamp
                };
                yield return forumThreadCreated;

                if (i % 3 == 0)
                {
                    var forumThreadEdited = new ForumThreadEdited
                    {
                        ForumThreadId =  forumThreadCreated.ForumThreadId,
                        EditorUserName = UserNames.Element(i),
                        Reason = "Fixed spelling errors",
                        Title = Titles.Element(i),
                        Content = LoremIpsum.Element(i),
                        Tags = new string[] { "c#", "asp.net-core", "entity-framework-core" },
                        TimeStamp = timestamp.AddMinutes(1.5)
                    };
                    yield return forumThreadEdited;
                }

                for (int j = rnd.Next(3); j < rnd.Next(15); j++)
                {
                    var forumPostCreated = new ForumItemPosted
                    {
                        ForumItemId =  Guid.NewGuid(),
                        ForumThreadId = forumThreadCreated.ForumThreadId,
                        AuthorUserName = UserNames.Element(i + j),
                        Content = LoremIpsum.Element(j + i),
                        TimeStamp = timestamp.AddSeconds(j * 20 + 5)
                    };
                    yield return forumPostCreated;

                    if (j % 4 == 0)
                    {
                        yield return new ForumItemEdited
                        {
                            ForumItemId = forumPostCreated.ForumItemId,
                            ForumThreadId = forumThreadCreated.ForumThreadId,
                            EditorUserName = UserNames.Element(i),
                            Content = LoremIpsum.Element(i + 3),
                            TimeStamp = forumPostCreated.TimeStamp.AddSeconds(j)
                        };
                    }
                }
            }
        }

        public IEnumerable<DomainEvent> Blog(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int backIndex = count - 1 - i;
                DateTime timestamp = DateTime.Now.AddMinutes(-(backIndex * 100) ^ 2 + 1);

                var created = new BlogLinkCreated
                {
                    BlogId = Guid.NewGuid(),
                    Url = "https://vyvojari.sk",
                    UserName = UserNames.Element(i),
                    Title = Titles.Element(i),
                    TimeStamp = timestamp
                };
                yield return created;
            }
        }
    }

    public static class Extentions
    {
        public static T Element<T>(this T[] array, int i) => array[i % array.Length];
    }
}
