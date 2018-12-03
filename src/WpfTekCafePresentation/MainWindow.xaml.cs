﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using LogicLayer;
using DataObjects;

namespace WpfTekCafePresentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// This Screen will be used by the Baristas and Developers.
    /// </summary>
    public partial class MainWindow : Window
    {
        private EmployeeManager _employeeManager = new EmployeeManager();
        private Employee _employee = null;
        private List<Project> _projects;
        private List<Project> _filteredProjects;
        private ProjectManager _projectManager = new ProjectManager();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void resetWindow()
        {
            _employee = null;

            btnLogin.Content = "Sign On";
            txtUserName.Visibility = Visibility.Visible;
            pwdPassword.Visibility = Visibility.Visible;
            txtUserName.Text = "Enter Email";
            pwdPassword.Password = "Enter Password";
            MessageBox.Content = "Welcome";
            Alert.Content = "Log in to Continue.";
            txtUserName.Focus();
            txtUserName.SelectAll();
            hideAllEmployeeTabs();

        }

        private void hideAllUserTabs()
        {
            foreach (TabItem item in tabViewMain.Items)
            {
                item.Visibility = Visibility.Collapsed;
            }
        }



        private void setupWindow()
        {
            //window for authorized login
            btnLogin.Content = "Log Out.";
            txtUserName.Visibility = Visibility.Hidden;
            pwdPassword.Visibility = Visibility.Hidden;
            txtUserName.Clear();
            pwdPassword.Clear();

            string name = _employee.FirstName + " " + _employee.LastName;
            Message.Content = name;

            string roles = "";

            for (int x = 0; x < _employee.Roles.Count; x++)
            {
                roles += _employee.Roles[x];
                if (x < _employee.Roles.Count - 2)
                {
                    roles += ", ";
                }
                else if (x < _employee.Roles.Count - 1)
                {
                    roles += " and ";
                }
            }
            Alert.Content = "Employee Role or Roles are:" + roles;

            showEmployeeTabs();
        }


        private void showEmployeeTabs()
        {
            // loop through the roles enabling tabs
            foreach (var tabView in _employee.Roles)
            {
                switch (tabView)
                {
                    case "SellTek":
                        tabSellTek.Visibility = Visibility.Visible;
                        tabSellTek.IsSelected = true;
                        break;
                    case "CheckOut":
                        tabCheckout.Visibility = Visibility.Visible;
                        tabCheckout.IsSelected = true;
                        break;
                    case "Reviews":
                        tabReviews.Visibility = Visibility.Visible;
                        tabReviews.IsSelected = true;
                        break;
                    case "HostSite":
                        tabHostSite.Visibility = Visibility.Visible;
                        tabHostSite.IsSelected = true;
                        break;
                    case "Develops":
                        tabDevelops.Visibility = Visibility.Visible;
                        tabDevelops.IsSelected = true;
                        break;
                    case "Developer":
                        tabDeveloper.Visibility = Visibility.Visible;
                        tabDeveloper.IsSelected = true;
                        break;
                    case "Admin":
                        tabAdmin.Visibility = Visibility.Visible;
                        tabAdmin.IsSelected = true;
                        break;
                    default:
                        break;
                }
            }
        }


        private void frmMain_Loaded(object sender, RoutedEventArgs e)
        {
            resetWindow();
        }

        private void txtUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUserName.SelectAll();
        }

        private void pwdPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            pwdPassword.SelectAll();
        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // login or logout depending on _employee state

            if (this._employee != null) // someone is already loggied in
            {
                resetWindow(); // log out
                return;
            }
            //Since, no one is logged in proceed...

            string employeeName = txtUserName.Text;
            string password = pwdPassword.Password;

            if (employeeName.Length > 255 || employeeName.Length < 7)
            {
                MessageBox.Show("Invalid Employee!");
                txtUserName.Focus();
                return;
            }
            if (password.Length < 6)
            {
                MessageBox.Show("Password Incorrect!");
                pwdPassword.Focus();
                return;
            }
            try
            {
                _employee = _employeerManager.AuthenticateUser(employeeName, password);
                if (_employee != null)
                {
                    MessageBox.Show(_employee.FirstName + ", you are authorized!");
                    if (_employee.Roles[0] == "Welcome New User")
                    {
                        this.Alert.Content = "You are logged in as " + _employee.Roles[0] + ". Update your password to continue.";

                        var chPassword = new frmUpdatePassword(_employee, _employeeManager, true);

                        if (chPassword.ShowDialog() == true)
                        {
                            setupWindow();
                        }
                    }
                    else
                    {
                        setupWindow();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);

            }
        }






        private void showRoles()
        {
            string roleString = "";
            foreach (var role in _employee.Roles)
            {
                roleString += role + " ";
            }
            this.Alert.Content = "You are logged in as: " + roleString;
        }

        private void tabSellTek_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                _projects = _projectManager.RetrieveProjectsByPhase("Intenent Products");
                if (cboProjectType.Items.Count == 0)
                {
                    var projectTypes = _projectManager.RetrieveProjectTypes();
                    foreach (var p in projectTypes)
                    {
                        cboBoatType.Items.Add(p);
                    }

                    cboProjectType.Items.Add("Show All");
                    cboProjectType.SelectedItem = "Show All";
                }
                if (_filteredProjects == null)
                {
                    _filteredProjects = _projects;
                }

                dgSellTek.ItemSource = _filteredProjects;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            filterSellTekGrid();
        }


        private void filterSellTekGrid()
        {
            try
            {
                int theWorkStation = int.Parse(txtCapacity.Text);
                _filteredProjects = _projects.FindAll(w => w.WorkStation >= theWorkStation);

                if (cboProjectType.SelectedItem.ToString() != "Show All")
                {
                    _filteredProjects = _filteredProjects.FindAll(w => w.ProjectTypeID == cboProjectType.SelectedItem.ToString());
                }
                dgSellTek.ItemSource = _filteredProjects;
            }
            catch (Exception)
            {
                MessageBox.Show("The WorkStation is a Number.");
                this.txtWorkStation.Text = "1";
                this.txtWorkStation.Focus();
                this.txtWorkStation.SelectAll();
                return;
            }
        }

        private viod btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            _filteredProjects = _projects;
            dgSellTek.ItemSource = _filteredProjects;
            txtWorkStation.Text = "1";
            cboProjectType.SelectedItem = "Show All";
        }

        private void cboProjectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _filteredProjectsGrid();
        }

        private void dgSellTek_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedProject = (Project)dgSellTek.SelectedItem;

            MessageBox.Show(selectedProject.Name);



            var detailView = new frmAddEditProject(selectedProject, DetailPurpose.SellTek);

            var result = detailView.ShowDialog();

            /////////////////////Add more here///////////////////////////////////////////////////////

        }

        private void dgSellTek_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dddd, MMMM dd, yyyy";
            }
            if (e.PropertyName == "WorkStation")
            {
                //finish this.
            };
        }

        private void tabManager_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                _projects = _projectManager.RetrieveProjectsByPhase();
                if (cboBoatType.Items.Count == 0)
                {
                    var projectType = _projectManager.RetrieveProjectTypes();
                    foreach (var p in projectTypes)
                    {
                        cboProjectType.Items.Add(p);
                    }

                    cboProjectType.Items.Add("Show All");
                    cboBoatType.SelectedItem = "Show All";

                }
                if (_filteredProjects == null)
                {
                    _filteredProjects = _projects;
                }
                dgManage.ItemsSource = _filteredProjects;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgManage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedProject = (Project)dgManage.SelectedItem;

            MessageBox.Show(selectedProject.Name);

            var detailView = new frmAddEditProject(selectedProject);
            //this pops up the detail window..
            var result = detailView = detailView.ShowDialog();
            if (result == true)
            {
                try
                {
                    this._filteredProjects = null;
                    this._projects = null;
                    _projects = _projectManager.GetBoatsByPhase();
                    if (cboBoatType.Items.Count == 0)
                    {
                        var projectTypes = _projectManager.GetProjectTypes();
                        foreach (var p in projectTypes)
                        {
                            cboProjectType.Items.Add(p);
                        }

                        cboProjectType.Items.Add("Show All");
                        cboBoatType.SelectedItem = "Show All";
                    }
                    if (_filteredProjects == null)
                    {
                        _filteredProjects = _projects;
                    }

                    dgManage.ItemSource = _filteredProjects;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }


    }
}
  