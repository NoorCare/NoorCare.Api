using AngularJSAuthentication.API.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class LikeVisitorController : ApiController
    {
        ILikeVisitorRepository _likeVisitorRepo = RepositoryFactory.Create<ILikeVisitorRepository>(ContextTypes.EntityFramework);

        [HttpGet]
        [Route("api/likevisitor/get/{clientId}/{hfpdocId}")]
        [AllowAnonymous]
        public IHttpActionResult getLikeVisitor(string clientId, string hfpdocId)
        {
            LikeVisitor _likeVisitor = _likeVisitorRepo.Find(x => x.NoorId.Trim() == clientId.Trim() && x.HFP_DOC_NCID.Trim() == hfpdocId.Trim() && x.IsDelete == true).FirstOrDefault();

            return Ok(_likeVisitor);
        }

        [HttpPost]
        [Route("api/likevisitor/save")]
        [AllowAnonymous]
        public IHttpActionResult saveLikeVisitor([FromBody] LikeVisitor likeVisitor)
        {
            if (likeVisitor != null)
            {
                LikeVisitor _likeVisitor = _likeVisitorRepo.Find(x => x.NoorId == likeVisitor.NoorId && x.HFP_DOC_NCID == likeVisitor.HFP_DOC_NCID && x.IsDelete==false).FirstOrDefault();
                if (_likeVisitor != null)
                {
                    //update
                    _likeVisitor.Like_Dislike = likeVisitor.Like_Dislike;
                    _likeVisitor.Visitor = likeVisitor.Visitor;
                    _likeVisitor.ModifyBy = likeVisitor.ModifyBy;
                    _likeVisitor.ModifyDate = DateTime.Now;
                    _likeVisitor.IsDelete = likeVisitor.IsDelete;
                    var result = _likeVisitorRepo.Update(_likeVisitor);
                    return Ok(result);

                }
                else
                {
                    //insert 
                    likeVisitor.CreateDate = DateTime.Now;
                    likeVisitor.ModifyDate = DateTime.Now;
                    var _likeVisitorCreated = _likeVisitorRepo.Insert(likeVisitor);
                    return Ok(_likeVisitorCreated);
                }
            }

            return InternalServerError();
        }

    }
}
