﻿using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Commander.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                var client = new LibrarianSephirahService.LibrarianSephirahServiceClient(GlobalContext.GrpcChannel);
                var (accessToken, refreshToken) = await GlobalContext.LibrarianClientService.GetTokenAsync(client, usernameTextBox.Text, passwordTextBox.Text);
                if (accessToken == null || refreshToken == null)
                {
                    throw new Exception("登录失败");
                }
                Core.GlobalContext.AccessToken = accessToken;
                Core.GlobalContext.RefreshToken = refreshToken;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                                $"发生异常：{ex.Message}",
                                "运行时错误",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private async void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                var client = new LibrarianSephirahService.LibrarianSephirahServiceClient(GlobalContext.GrpcChannel);
                if (string.IsNullOrEmpty(Core.GlobalContext.RefreshToken) == false)
                {
                    var (accessToken, refreshToken) = await GlobalContext.LibrarianClientService.GetTokenAsync(client);
                    if (accessToken != null && refreshToken != null)
                    {
                        Core.GlobalContext.AccessToken = accessToken;
                        Core.GlobalContext.RefreshToken = refreshToken;
                        MessageBox.Show(this, "已使用RefreshToken登录");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                                $"发生异常：{ex.Message}",
                                "运行时错误",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }
    }
}
