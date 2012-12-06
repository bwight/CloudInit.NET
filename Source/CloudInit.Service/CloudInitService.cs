//-----------------------------------------------------------------------
// <copyright file="CloudInitService.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.ServiceProcess;
using CloudInit.Modules.Core;
using StructureMap;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CloudInit
{
    public partial class CloudInitService : ServiceBase
    {
        public CloudInitService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            ThreadPool.QueueUserWorkItem(CIService.Main, this);
        }

        protected override void OnStop()
        {

        }
    }
}
