namespace Subtitle.Downloader.Tests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using NUnit.Framework;
    using Provider.Addic7ed;

    [TestFixture]
    public class WhenLookingForTheBestSubtitleVersion
    {
        [Test]
        public void IfOneVersionPresentAndItMatchesReturnThatDownloadLink()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media {Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv"};

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Link.Link.Should().Be("Found Link");
        }

        [Test]
        public void IfAVersionIsFoundPassTheLanguageInTheResult()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media { Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv" };

            var foundLink = SubtitleSearch.For(media, versions, new List<string> { "Dutch", "English" });

            foundLink.Subtitle.Language.Should().Be("English");
        }

        [Test]
        public void OnlyCompleteSubtitlesShouldBeDownloaded()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "Dutch",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Link", Type = SubtitleLinkType.Download}
                            },
                            Completed = false
                        }
                    }
                }
            };

            var media = new Media { Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv" };

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Should().BeNull();
        }

        [Test]
        public void OnlyEnglishOrDutchSubtitlesShouldBeDownloaded()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "Other Language",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media {Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv"};

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Should().BeNull();
        }

        [Test]
        public void IfTwoVersionsPresentAndTheyBothMatchDownloadTheMostUpdatedOne()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Download Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                },
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Updated Link", Type = SubtitleLinkType.Updated}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media {Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv"};

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Link.Link.Should().Be("Found Updated Link");
        }

        [Test]
        public void IfTwoVersionsPresentAndTheyBothMatchDownloadTheMostDownloaded()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Downloads = 100,
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Most Downloaded Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                },
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Downloads = 1,
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Least Downloaded Link", Type = SubtitleLinkType.Updated}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media {Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv"};

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Link.Link.Should().Be("Found Most Downloaded Link");
        }

        [Test]
        public void IfTwoVersionsPresentAndTheyBothMatchDownloadTheNonHearingImpairedVersion()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            HearingImpaired = false,
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink
                                {
                                    Link = "Found Non Hearing Impaired Link",
                                    Type = SubtitleLinkType.Download
                                }
                            }
                            , Completed = true
                        }
                    }
                },
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            HearingImpaired = true,
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Hearing Impaired Link", Type = SubtitleLinkType.Updated}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media {Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv"};

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Link.Link.Should().Be("Found Non Hearing Impaired Link");
        }

        [Test]
        public void IfTwoVersionsPresentOneEnglishAndOnDutchDownloadOnlyDutch()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found English Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                },
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "Dutch",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Dutch Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media {Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv"};

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Link.Link.Should().Be("Found Dutch Link");
        }

        [Test]
        public void IfTwoVersionsPresentOneEnglishAndOenDutchDownloadBasedOnOrder()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found English Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                },
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "German",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found German Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media { Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv" };

            var foundLink = SubtitleSearch.For(media, versions, new List<string> { "German", "English" });

            foundLink.Link.Link.Should().Be("Found German Link");
        }

        [Test]
        public void IfOneVersionPresentWithTwoLinksReturnMostUpdatedLink()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink
                                {
                                    Link = "Found Download Link",
                                    Type = SubtitleLinkType.Download
                                },
                                new SubtitleLink
                                {
                                    Link = "Found Updated Link",
                                    Type = SubtitleLinkType.Updated
                                }
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media {Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv"};

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Link.Link.Should().Be("Found Updated Link");
        }

        [Test]
        public void IfOneVersionPresentWithBothEnglishAndDutchSubtitleReturnDutchLink()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "KILLERS",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found English Link", Type = SubtitleLinkType.Download}
                            }
                        },
                        new Subtitle
                        {
                            Language = "Dutch",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Dutch Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media {Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv"};

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Link.Link.Should().Be("Found Dutch Link");
        }

        [Test]
        public void IfOneVersionIsPresentButTheReleaseDoesntMatchReturnAnEmptyString()
        {
            var versions = new List<SubtitleVersion>
            {
                new SubtitleVersion
                {
                    Release = "OTHER",
                    Subtitles = new List<Subtitle>
                    {
                        new Subtitle
                        {
                            Language = "English",
                            Links = new List<SubtitleLink>
                            {
                                new SubtitleLink {Link = "Found Link", Type = SubtitleLinkType.Download}
                            }
                            , Completed = true
                        }
                    }
                }
            };

            var media = new Media {Name = "ShowName.S01E02.720p.HDTV-KILLERS.mkv"};

            var foundLink = SubtitleSearch.For(media, versions, new List<string> {"Dutch", "English"});

            foundLink.Should().BeNull();
        }
    }
}