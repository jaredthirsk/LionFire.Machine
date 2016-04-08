using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using LionFire.RevisionControl;
using Microsoft.Extensions.OptionsModel;
using LionFire.PullAgent;
using System.IO;
using LionFire.Machine.Services;

namespace LionFire.Web.PullAgent.Mvc.Controllers
{


    public class GogsController : Controller
    {
        #region Injected

        PullAgentOptions config;
        IServiceControlService serviceControlService;

        #endregion

        #region Construction

        public GogsController(IOptions<PullAgentOptions> config, IServiceControlService serviceControlService)
        {
            this.config = config.Value;
            this.serviceControlService = serviceControlService;
        }

        #endregion

        [HttpGet]
        public string Pull(string name)
        {
            List<string> results = new List<string>();
            foreach (var kvp in config.Repositories.Where(_kvp => _kvp.Key == name))
            {
                results.Add((kvp.Value.Tag + kvp.Value.Branch) + ": " + _PullRepoConfig(kvp.Key, kvp.Value, kvp.Value.Tag, kvp.Value.Branch));
            }
            if (results.Count == 0) { return "No repository found"; }

            var result = "Restarting " + name + ": " + serviceControlService.Restart(name);

            return results.Aggregate((x, y) => x + Environment.NewLine + y);
        }

        [HttpGet]
        public List<string> PullAll()
        {
            return _Pull();
        }

        [HttpPost]
        public List<string> Index([FromBody] dynamic json)
        {
            string repoUri = json.repository.url;

            return _Pull(new Func<KeyValuePair<string, RepoOptions >,bool>( _kvp => _kvp.Value.Url == repoUri), json);
        }

        internal  List<string> _Pull(Func<KeyValuePair<string, RepoOptions>, bool> filter = null, dynamic json=null)
        {
            var results = new List<string>();
            if (filter == null) filter = _ => true;
            foreach (var kvp in config.Repositories.Where(filter))
            {
                var repoOptions = kvp.Value;
                var tag = String.IsNullOrWhiteSpace(repoOptions.Tag) ? null : repoOptions.Tag;
                if (tag != null
                    && (json?.ref_type?.ToString() != "tag"
                        || json?.@ref?.ToString() != repoOptions.Tag)
                    )
                {
                    continue;
                }

                var branch = String.IsNullOrWhiteSpace(repoOptions.Branch) ? null : repoOptions.Branch;
                if (branch != null
                    && (
                        //json.ref_type?.ToString() != "branch"|| 
                        json?.@ref?.ToString() != "refs/heads/" + repoOptions.Branch)
                    )
                {
                    continue;
                }

                results.Add(_PullRepoConfig(kvp.Key, kvp.Value, tag, branch));
            }
            return results;
        }

        #region (Private) Methods

        private string _PullRepoConfig(string name, RepoOptions repoOptions, string tag = null, string branch = null)
        {

            //var pullUrl = json.repository.ssh_url as string ?? json.repository.url as string;
            var pullUrl = repoOptions.SshUrl ?? repoOptions.Url;

            bool result = Git.Pull(repoOptions.Path, pullUrl);

            if (result)
            {
                if (tag != null)
                {
                    result &= Git.CheckoutTag(repoOptions.Path, tag);
                }
                else if (branch != null)
                {
                    result &= Git.CheckoutBranch(repoOptions.Path, branch);
                }
            }
            return result.ToString();
        }

        #endregion

    }
}
