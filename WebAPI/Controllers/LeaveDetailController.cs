using Microsoft.Ajax.Utilities;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class LeaveDetailController : ApiController
    {
        ILeaveDetailRepository _leaveDtlRepo = RepositoryFactory.Create<ILeaveDetailRepository>(ContextTypes.EntityFramework);
        ILeaveMasterRepository _leaveMasterRepo = RepositoryFactory.Create<ILeaveMasterRepository>(ContextTypes.EntityFramework);
        ITimeMasterRepository _timeMasterRepo = RepositoryFactory.Create<ITimeMasterRepository>(ContextTypes.EntityFramework);
       

        [Route("api/leavedetail/{clientId}")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult getLeaveDetail(string clientId)
        {
            //var leaveDtls = _leaveDtlRepo.GetAll().Where(x => x.ClientId == clientId && x.IsDeleted == false).ToList();
            var leaveMaster = _leaveMasterRepo.GetAll().Where(x => x.IsDeleted == false);

            var result = (from d in _leaveDtlRepo.GetAll().Where(x => x.ClientId == clientId && x.IsDeleted == false)
                          select new
                          {
                              Id = d.Id,
                              ClientId = d.ClientId,
                              LeaveId = d.LeaveId,
                              LeaveType = (from h in leaveMaster where (h.LeaveId == d.LeaveId) select h.LeaveType).FirstOrDefault(),
                              FromDate = d.FromDate,
                              ToDate = d.ToDate,
                              TimeId = d.TimeId,
                              Remarks = d.Remarks,
                              IsDeleted = d.IsDeleted,
                              TimeIds= getDocAvailablity(d.TimeId)
                          }).ToList();
            return Ok(result);
        }

        private List<DoctorScheduleTime> getDocAvailablity(string LeaveId)
        {
            if (LeaveId == null || LeaveId == "")
            {
                return new List<DoctorScheduleTime>();
            }
            var leaveIds = LeaveId.Split(',');
            int[] myInts = Array.ConvertAll(leaveIds, s => int.Parse(s));
            var timeList = _timeMasterRepo.GetAll().Where(x => myInts.Contains(x.Id)).ToList();
            List<DoctorScheduleTime> doctorTimes = new List<DoctorScheduleTime>();
            foreach (var item in timeList)
            {
                DoctorScheduleTime obj = new DoctorScheduleTime();
                obj.TimeId = item.Id;
                obj.TimeDesc = item.TimeFrom.Trim() + ' ' + item.TimeTo.Trim() + ' ' + item.AM_PM.Trim();
                doctorTimes.Add(obj);
            } 

            return doctorTimes;
        }

        [HttpPost]
        [Route("api/leavedetail/manage")]
        [AllowAnonymous]
        public IHttpActionResult manageLeaves(LeaveDetail leaveDetail)
        {
            int id = 0;
            if (leaveDetail != null)
            {
                if (leaveDetail.Id == 0)
                {
                    //add
                    leaveDetail.Createdby = leaveDetail.ClientId;
                    leaveDetail.CreatedDate = DateTime.Now;
                    leaveDetail.ModifyBy = leaveDetail.ClientId;
                    leaveDetail.ModifyDate = DateTime.Now;
                    leaveDetail.IsDeleted = false;

                    return Ok(_leaveDtlRepo.Insert(leaveDetail));
                }
                else
                {
                    //update
                    LeaveDetail _leave = _leaveDtlRepo.Find(x => x.Id == leaveDetail.Id).FirstOrDefault();
                    if (_leave != null)
                    {
                        _leave.FromDate = leaveDetail.FromDate;
                        _leave.ToDate = leaveDetail.ToDate;
                        _leave.TimeId = leaveDetail.TimeId;
                        _leave.LeaveId = leaveDetail.LeaveId;
                        _leave.Remarks = leaveDetail.Remarks;
                        _leave.ModifyBy = leaveDetail.ClientId;
                        _leave.ModifyDate = DateTime.Now;
                        return Ok(_leaveDtlRepo.Update(_leave));
                    }

                }
            }
            return Ok(false);
        }


        [HttpPost]
        [Route("api/leavedetail/delete")]
        public IHttpActionResult deleteLeaves(LeaveDetail leaveDetail)
        {
            LeaveDetail _leave = _leaveDtlRepo.Find(x => x.Id == leaveDetail.Id).FirstOrDefault();
            _leave.IsDeleted = leaveDetail.IsDeleted;
            _leave.ModifyBy = leaveDetail.ClientId;
            _leave.ModifyDate = DateTime.Now;
            return Ok(_leaveDtlRepo.Update(_leave));
        }
    }
}
