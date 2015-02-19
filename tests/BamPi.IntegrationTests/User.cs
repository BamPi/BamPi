using System;
using System.Collections.Generic;

namespace BamPi.IntegrationTests
{
    public class User
    {
        public User()
        {
            Groups = new List<Group>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Group> Groups { get; set; }
    }
}