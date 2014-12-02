using System;
using iyates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NOSJ.Tests
{
    [TestClass]
    public class NosjTests
    {
        [TestMethod]
        public void Nosj_pivots_Json()
        {
            var o = new
            {
                One = 1,
                Two = "B",
                Three = true,
                Four = 123.456
            };

            var result = Nosj.Serialize(o);

            Assert.AreEqual("[[\"One\",\"Two\",\"Three\",\"Four\"],[1,\"B\",true,123.456]]", result);
        }

        [TestMethod]
        public void Nosj_minimises_nulls()
        {
            var o = new
            {
                One = 1,
                Two = "B",
                Three = true,
                Four = 123.456,
                Five = (object)null
            };

            var result = Nosj.Serialize(o);

            Assert.AreEqual("[[\"One\",\"Two\",\"Three\",\"Four\"],[1,\"B\",true,123.456]]", result);
        }

        [TestMethod]
        public void Nosj_minimises_defaults()
        {
            var o = new
            {
                One = 1,
                Two = "B",
                Three = false,
                Four = 123.456
            };

            var result = Nosj.Serialize(o);

            Assert.AreEqual("[[\"One\",\"Two\",\"Four\"],[1,\"B\",123.456]]", result);
        }

        [TestMethod]
        public void Nosj_can_pivot_array()
        {
            var o = new[]
            {
                new {
                    One = 1,
                    Two = "B",
                    Three = true,
                    Four = 123.456
                },
                new {
                    One = 2,
                    Two = "C",
                    Three = false,
                    Four = 456.123
                }
            };

            var result = Nosj.Serialize(o);

            Assert.AreEqual("[[\"One\",\"Two\",\"Three\",\"Four\"],[1,\"B\",true,123.456],[2,\"C\",false,456.123]]", result);
        }

        [TestMethod]
        public void Nosj_includes_deep_objects()
        {
            var o = new[]
            {
                new {
                    One = 1,
                    Two = "B",
                    Three = true,
                    Four = 123.456,
                    Five = (object)new {
                        A = "Aaa"
                    }
                },
                new {
                    One = 2,
                    Two = "C",
                    Three = false,
                    Four = 456.123,
                    Five = (object)null
                }
            };

            var result = Nosj.Serialize(o);

            Assert.AreEqual("[[\"One\",\"Two\",\"Three\",\"Four\",\"Five\"],[1,\"B\",true,123.456,{\"A\":\"Aaa\"}],[2,\"C\",false,456.123]]", result);
        }

        [TestMethod]
        public void Supports_different_objects()
        {
            var o = new object[]
            {
                new {
                    One = 1,
                    Two = "B",
                    Three = true
                },
                new {
                    Four = 456.123,
                    Five = new { This = "That" }
                }
            };

            var result = Nosj.Serialize(o);

            Assert.AreEqual("[[\"One\",\"Two\",\"Three\",\"Four\",\"Five\"],[1,\"B\",true],[\"__UNDEF__\",\"__UNDEF__\",\"__UNDEF__\",456.123,{\"This\":\"That\"}]]", result);
        }

        [TestMethod]
        public void Missing_value_treated_differently_to_default()
        {
            var o = new object[]
            {
                new {
                    Bool = false,
                    Optional = 123,
                    False = false
                },
                new {
                    Bool = true,
                    False = false
                },
                new {
                    Bool = true,
                    Different = 456,
                    NotUsed = (object)null,
                    False = false
                },
            };

            var result = Nosj.Serialize(o);

            Assert.AreEqual("[[\"Optional\",\"Bool\",\"Different\"],[123,false],[\"__UNDEF__\",true],[\"__UNDEF__\",true,456]]", result);
        }

        [TestMethod]
        public void Can_use_null_instead_of_UNDEF()
        {
            var o = new object[]
            {
                new {
                    Bool = false,
                    Optional = 123,
                    False = false
                },
                new {
                    Bool = true,
                    False = false
                },
                new {
                    Bool = true,
                    Different = 456,
                    NotUsed = (object)null,
                    False = false
                },
            };

            var result = Nosj.Serialize(o, nullMissing: true);

            Assert.AreEqual("[[\"Optional\",\"Bool\",\"Different\"],[123,false],[null,true],[null,true,456]]", result);
        }

        [TestMethod]
        public void Can_show_all_fields()
        {
            var o = new object[]
            {
                new {
                    Bool = false,
                    Optional = 123,
                    False = false
                },
                new {
                    Bool = true,
                    False = false
                },
                new {
                    Bool = true,
                    Different = 456,
                    NotUsed = (object)null,
                    False = false
                },
            };

            var result = Nosj.Serialize(o, stripDefaults: false);

            Assert.AreEqual("[[\"Bool\",\"Optional\",\"False\",\"Different\"],[false,123,false],[true,\"__UNDEF__\",false],[true,\"__UNDEF__\",false,456]]", result);
        }
    }
}
