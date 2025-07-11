using AutoMapper;
using log4net;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CouponAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while fetching coupons.", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(x => x.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while fetching coupon with ID: {id}", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpGet("GetByCode/{code}")]
        public ResponseDto Get(string code)
        {
            try
            {
                Coupon obj = _db.Coupons.First(
                    x => x.CouponCode.Equals(code, StringComparison.CurrentCultureIgnoreCase));

                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while fetching coupon with code: {code}", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Add(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while creating a new coupon.", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                var existingCoupon = _db.Coupons.Find(couponDto.CouponId) ?? throw new KeyNotFoundException($"Coupon with CouponId {couponDto.CouponId} not found.");

                Coupon obj = _mapper.Map(couponDto, existingCoupon); // This updates only mapped fields
                _db.Coupons.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while updating coupon with ID {couponDto.CouponId}", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(x => x.CouponId == id);
                _db.Coupons.Remove(obj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while deleting coupon with ID {id}", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}