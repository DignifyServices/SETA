using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SETA.Generic
{
    /// <summary>
    /// Prod, Preview, Test
    /// </summary>
    public enum Environments
    {
        test,
        preview,
        prod
    }

    /// <summary>
    /// MP1, MP2, MP3
    /// </summary>
    public enum EnvironmentsMP
    {
        MP1,
        MP2,
        MP3
    }

    public enum Brands
    {
        GVA,
        HBVL,
        NB,
        DS,
        LIMNL
    }

    public enum Webmethods
    {
        GET,
        POST
    }
}