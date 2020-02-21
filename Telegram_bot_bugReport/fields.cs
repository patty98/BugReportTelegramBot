using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_bot_bugReport
{
    public class IssueClass
    {
        public fields fields = new fields();
    }
    public class fields
    {
        public Project project;

        public string summary { get; set; }
        public string description { get; set; }
        public Assignee assignee;
        public Issuetype issuetype = new Issuetype();
    }

        public class Assignee
        {
            public string name { get; set; } = "";
            public Assignee()
            {
                name = "";
            }
        }
        public class Issuetype
        {
            public string name { get; set; }

            public Issuetype()
            {
                name = "Bug";
            }

        }
        public class Project
        {
            public string key { get; set; }
            public Project()
            {
                key = "";
            }
        }
    



    //public class IssueClass
    //{
    //public Fields fields = new Fields();
    //}

    //public class Fields
    //{
    //    public Project project { get; set; }
    //    public string summary { get; set; }
    //    public string description { get; set; }
    //    public Assignee assignee { get; set; }
    //    public IssueType issuetype { get; set; }
    //    public Fields()
    //    {
    //        project = new Project();
    //        issuetype = new IssueType();
    //    }
    //}

    //public class Project
    //{
    //    public string key { get; set; }
    //}

    //public class IssueType
    //{
    //    public string name { get; set; }
    //}
    //public class Assignee
    //{
    //    public string name { get; set; }
    //}
}


