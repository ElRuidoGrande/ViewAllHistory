using EnvDTE;
using TFSExtensions.ViewAllHistory.Model;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TFSExtensions.ViewAllHistory
{
    /// <summary>
    /// Interaction logic for HistoryControl.xaml
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        #region [Internal Properties]

        internal DTE Application { get; set; }

        internal Workspace Workspace { get; set; }

        internal string InitialPath { get; set; }

        #endregion

        public HistoryControl()
        {
            InitializeComponent();
        }        

        private string GetProjectName(string path)
        {
            var spilt = path.Split('/');

            if (spilt.Length > 1)
            { 
                return spilt[1];
            }

            return spilt[0];
        }


        IEnumerable<Changeset> GetChangesets(Workspace workspace, string path)
        {
            return workspace.VersionControlServer.QueryHistory(path, RecursionType.OneLevel);
        }

        string GetNewPath(ItemIdentifier branch, string path)
        {
            var newSplit = branch.Item.Split('/');
            var oldSplit = path.Split('/');

            if (oldSplit[0] != null && newSplit[0] != null)
            {
                oldSplit[0] = newSplit[0];
            }

            if (oldSplit[1] != null && newSplit[1] != null)
            {
                oldSplit[1] = newSplit[1];
            }

            if (oldSplit[2] != null && newSplit[2] != null)
            {
                oldSplit[2] = newSplit[2];
            }

            StringBuilder newPath = new StringBuilder();
            for (int counter = 0; counter < oldSplit.Length; counter++)
            {
                newPath.Append(oldSplit[counter]);

                if (counter < oldSplit.Length - 1)
                {
                    newPath.Append('/');
                }
            }

            return newPath.ToString();
        }


        private ResultModel GetLatestChangeset()
        {
            List<ResultModel> results = this.resultGrid.ItemsSource as List<ResultModel>;

            return results.OrderByDescending(r => r.ChangesetDate).FirstOrDefault();
        }

        public List<BranchObject> GetBranches(Workspace workspace, string projectName)
        {
            var rootBranches =  workspace.VersionControlServer.QueryRootBranchObjects(RecursionType.Full);

            return rootBranches.Where(b => b.Properties.RootItem.Item.Contains(projectName)).ToList();
        }

        public void DisplayHistory(DTE application, Workspace workspace, string path)
        {
            if (this.resultGrid.ItemsSource != null)
            {
                this.resultGrid.ItemsSource = null;
                this.resultGrid.Items.Refresh();
            }

            this.Application = application;
            this.Workspace = workspace;
            this.InitialPath = path;

            BackgroundWorker historyWorker = new BackgroundWorker();
            historyWorker.DoWork += historyWorker_DoWork;
            historyWorker.RunWorkerCompleted += historyWorker_RunWorkerCompleted;
            historyWorker.RunWorkerAsync();         
        }


      
        void historyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            lock(this.Workspace)
            lock (this.InitialPath)
            {
                List<ResultModel> results = new List<ResultModel>();

                string project = GetProjectName(this.InitialPath);
                List<BranchObject> branches = GetBranches(this.Workspace, project);

                foreach (var branch in branches)
                {
                    string rootPath = GetNewPath(branch.Properties.RootItem, this.InitialPath);
                    var rootResults = GetChangesets(this.Workspace, rootPath);                  

                    if (rootResults != null && rootResults.Any())
                    {
                        rootResults.ToList().ForEach(result => results.Add(new ResultModel()
                        {
                            Branch = branch.Properties.RootItem.Item,
                            ChangesetId = result.ChangesetId,
                            Note = result.Comment,
                            ChangesetDate = result.CreationDate,
                            Committer = result.Committer
                        }));
                    }

                }

                results = results.OrderByDescending(r => r.ChangesetDate).ToList();
                e.Result = results;
            }
        }

        void historyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<ResultModel> results = e.Result as List<ResultModel>;
            this.resultGrid.ItemsSource = results;
        }


        private void resultGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var result = this.resultGrid.SelectedItem as ResultModel;

            VersionControlExt vce;
                      
            vce = this.Application.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            vce.ViewChangesetDetails(result.ChangesetId);
        }

    }
}
