using System;
using System.Collections.Generic;

namespace BamPi.IntegrationTests
{
    public class Group
    {
        public Group()
        {
            Users = new List<User>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}