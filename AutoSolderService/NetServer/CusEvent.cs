

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoSolderService
{
    #region Network Connect Event
    /// <summary>
    /// Network Connect Args
    /// </summary>
    public class NetConnectArgs : EventArgs
    {
        public readonly NetConnectStat NetStat;
        public NetConnectArgs(NetConnectStat netstat)
        {
            NetStat = netstat;
        }
    }
    /// <summary>
    /// Network Connect delegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void NetConnectHandler(object sender, NetConnectArgs e);

    #endregion

    #region Network Recive Data Event
    /// <summary>
    /// Network Recive Data Args
    /// </summary>
    public class ReciveDataArgs : EventArgs
    {
        public readonly byte[] ReciveData;
        public ReciveDataArgs(byte[] recivedata)
        {
            ReciveData = recivedata;
        }
    }
    /// <summary>
    /// Network Recive Data delegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ReciveDataHandler(object sender, ReciveDataArgs e);

    /// <summary>
    /// Network display data to form
    /// </summary>
    //public class DisplayToUIArgs : EventArgs
    //{
    //    public readonly BaseProfile Baseprofile;
    //    public DisplayToUIArgs(BaseProfile baseprofile)
    //    {
    //        Baseprofile = baseprofile;
    //    }
    //}
    /// <summary>
    /// Network display data delegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //public delegate void DisplayToUIHandler(object sender, DisplayToUIArgs e);
    #endregion
}
