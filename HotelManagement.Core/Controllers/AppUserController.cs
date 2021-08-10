using AutoMapper;
using HotelManagement.Data;
using HotelManagement.Data.Dtos;
using HotelManagement.Data.Models;
using HotelManagement.Data.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace HotelManagement.Core.Controllers
{
    /// <summary>
    /// This controller handles everything associated with the AppUser
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;
        private readonly HotelManagementDbContext _dbContext;

        /// <summary>
        /// This is the constructor where initialization occurs
        /// Depency injection is done here
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="mapper"></param>
        /// <param name="roomRepository"></param>
        /// <param name="dbContext"></param>
        public AppUserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, 
            IMapper mapper, IRoomRepository roomRepository, HotelManagementDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _roomRepository = roomRepository;
            _dbContext = dbContext;
        }

        
        /// <summary>
        /// This endpoint registers a user and makes the user a Guest
        /// </summary>
        /// <param name="model">This model contains the user's information</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /api/appuser/add-guest
        ///
        /// </remarks>
        /// <response code="201">If the user is successfully registered</response>
        /// <response code="400">If the user did not fill the form correctly OR if the user already exists in the database</response>
        /// <response code="500">If the system could not create the user OR if the system could not make the user a "Guest"</response>
        [HttpPost("add-guest", Name = nameof(AddGuest))]
        //[Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddGuest([FromBody] AppUserDto model)
        {
            // check if the form is filled correctly
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("model", "PLease fill out the form correctly");
                return BadRequest(ModelState);
            }

            // check if user already exists
            AppUser checkUser = await _userManager.FindByEmailAsync(model.Email);
            if (checkUser != null)
            {
                ModelState.AddModelError("model", "User already exists");
                return BadRequest(ModelState);
            }

            // add user
            AppUser incommingUser = _mapper.Map<AppUser>(model);
            incommingUser.UserName = model.Email;
            IdentityResult result =  await _userManager.CreateAsync(incommingUser, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                    return StatusCode(500, ModelState);
                }
            }

            // check if role (Guest) exists
            if (!await _roleManager.RoleExistsAsync("Guest"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Guest"));
            }

            // add user to role
            IdentityResult roleResult = await _userManager.AddToRoleAsync(incommingUser, "Guest");
            if (!roleResult.Succeeded)
            {
                foreach (var err in roleResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return StatusCode(500, ModelState);
            }


            return CreatedAtAction("AddGuest", incommingUser);
        }

        /// <summary>
        /// This endpoint registers a user and makes the user a SuperAdmin
        /// </summary>
        /// <param name="model">This model contains the user's information</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /api/appuser/add-superadmin
        ///
        /// </remarks>
        /// <response code="201">If the user is successfully registered</response>
        /// <response code="400">If the user did not fill the form correctly OR if the user already exists in the database</response>
        /// <response code="500">If the system could not create the user OR if the system could not make the user a "Guest"</response>

        [HttpPost("add-superadmin", Name = nameof(AddSuperAdmin))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddSuperAdmin([FromBody] AppUserDto model)
        {
            // check if the form is filled correctly
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("model", "PLease fill out the form correctly");
                return BadRequest(ModelState);
            }

            // check if user already exists
            AppUser checkSuperAdmin = await _userManager.FindByEmailAsync(model.Email);
            if (checkSuperAdmin != null)
            {
                ModelState.AddModelError("model", "User already exists");
                return BadRequest(ModelState);
            }

            // add user
            AppUser incommingSuperAdmin = _mapper.Map<AppUser>(model);
            incommingSuperAdmin.UserName = model.Email;
            IdentityResult result = await _userManager.CreateAsync(incommingSuperAdmin, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                    return StatusCode(500, ModelState);
                }
            }

            // check if role (SuperEmployee) exists
            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }

            // add user to role
            IdentityResult roleResult = await _userManager.AddToRoleAsync(incommingSuperAdmin, "SuperAdmin");
            if (!roleResult.Succeeded)
            {
                foreach (var err in roleResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return StatusCode(500, ModelState);
            }


            return CreatedAtAction("AddSuperAdmin", incommingSuperAdmin);
        }


        /// <summary>
        /// This endpoint registers a user and makes the user an Employee.
        /// It can only be accessed by a SuperAdmin
        /// </summary>
        /// <param name="model">This model contains the user's information</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /api/appuser/add-employee
        ///
        /// </remarks>
        /// <response code="201">If the user is successfully registered</response>
        /// <response code="400">If the user did not fill the form correctly OR if the user already exists in the database</response>
        /// <response code="500">If the system could not create the user OR if the system could not make the user a "Guest"</response>

        [HttpPost("add-employee", Name = nameof(AddEmployee))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AddEmployee([FromBody] AppUserDto model)
        {
            // check if current logged-in-user is a SuperAdmin
            AppUser currentLoggedInUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentLoggedInUser, "SuperAdmin"))
            {
                ModelState.AddModelError("Access-denied", "Unauthorized User");
                return Unauthorized(ModelState);
            }

            // check if the form is filled correctly
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("model", "PLease fill out the form correctly");
                return BadRequest(ModelState);
            }

            // check if user already exists
            AppUser checkEmployee = await _userManager.FindByEmailAsync(model.Email);
            if (checkEmployee != null)
            {
                ModelState.AddModelError("model", "User already exists");
                return BadRequest(ModelState);
            }

            // add user
            AppUser incommingEmployee = _mapper.Map<AppUser>(model);
            incommingEmployee.UserName = model.Email;
            IdentityResult result = await _userManager.CreateAsync(incommingEmployee, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                    return BadRequest(ModelState);
                }
            }

            // check if role (Employee) exists
            if (!await _roleManager.RoleExistsAsync("Employee"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Employee"));
            }

            // add user to role
            IdentityResult roleResult = await _userManager.AddToRoleAsync(incommingEmployee, "Employee");
            if (!roleResult.Succeeded)
            {
                foreach (var err in roleResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return BadRequest(ModelState);
            }


            return CreatedAtAction("AddEmployee", incommingEmployee);
        }


        /// <summary>
        /// This endpoint books a room.
        /// It can only be accessed by a logged-in-user
        /// </summary>
        /// <param name="model">This model contains the room number picked by the user and the duration</param>
        /// <param name="id">This is the id of the currently-logged-in-user</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /api/appuser/book-room/roomNumber
        ///
        /// </remarks>
        /// <response code="201">If the user successfully books the book</response>
        /// <response code="401">If the currently logged in user's id is different from the id passed in from the URL</response>
        /// <response code="400">If the roomm number provided does not match any room number in the database OR If the given room is not available for booking</response>
        /// <response code="500">If no change was made to the database</response>

        [HttpPost("book-room/{roomNumber}", Name = nameof(BookRoom))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> BookRoom([FromBody] BookingDto model, string id)
        {
            // check if current logged-in user is the one sending the request
            AppUser currentLoggedInUser = await _userManager.GetUserAsync(User);
            if (currentLoggedInUser.Id != id)
            {
                ModelState.AddModelError("User", "Unauthorized");
                return Unauthorized(ModelState);
            }

            // get the room using the room number
            Room room = await _roomRepository.GetRoomByRoomNumber(model.RoomNumber);
            if (room == null)
            {
                ModelState.AddModelError("Room", "Requested room does not exist");
                return BadRequest(ModelState);
            }

            // check if the room is available
            var availableRooms = await _roomRepository.GetAvailableRooms();
            if (!availableRooms.Contains(room))
            {
                ModelState.AddModelError("Room", "Requested room is not available for booking. Please try booking other rooms");
                return BadRequest(ModelState);
            }

            // book room
            room.Availability = false;

            // update bookinghistory table
            BookingHistory bookings = new BookingHistory
            {
                RoomNumber = model.RoomNumber,
                AppUserId = currentLoggedInUser.Id,
                RoomId = room.RoomId,
                Duration = model.Duration,
                TimeOut = DateTime.Now.AddDays(model.Duration),
            };

            _dbContext.BookingHistoryTbl.Add(bookings);
            var result = await _dbContext.SaveChangesAsync();
            if (result < 1)
            {
                return StatusCode(500, "InternalServerError");
            }

            return CreatedAtAction("BookRoom", new { RoomDetails = room, Message = $"You have successfully booked room {room.RoomNumber}" });
        }

        /// <summary>
        /// This endpoint updates a booked room.
        /// It can only be accessed by a logged-in-user
        /// </summary>
        /// <param name="model">This model contains the room details the user wants to update</param>
        /// <param name="roomId">This is the id of the room the user wants to update</param>
        /// <param name="AppUserId">This is the id of the currently-logged-in-user</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /api/appuser/update-booking/roomId
        ///
        /// </remarks>
        /// <response code="200">If the user successfully updates the booking of the given room</response>
        /// <response code="401">If the currently logged in user's id is different from the id passed in from the URL</response>
        /// <response code="400">If the roomm number provided does not match any room number in the database OR If the given room is not yet booked</response>
        /// <response code="500">If no change was made to the database</response>

        [HttpPost("update-booking/{roomId}", Name = nameof(UpdateBooking))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> UpdateBooking([FromBody] BookingDto model, string roomId, string AppUserId)
        {
            // check if current logged-in user is the one sending the request
            AppUser currentLoggedInUser = await _userManager.GetUserAsync(User);
            if (currentLoggedInUser.Id != AppUserId)
            {
                ModelState.AddModelError("User", "Unauthorized");
                return Unauthorized(ModelState);
            }

            // get the room using the room number
            Room room = await _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                ModelState.AddModelError("Room", "Requested room does not exist");
                return BadRequest(ModelState);
            }

            bool isRoomBooked = await _roomRepository.IsRoomBooked(roomId);
            if (isRoomBooked == true)
            {
                ModelState.AddModelError("Room", "Requested room is not yet booked");
                return BadRequest(ModelState);
            }
            var roomBookingHistory = room.BookingHistory.ToList();
            var lastBookingHistory = roomBookingHistory.LastOrDefault();

            lastBookingHistory.Duration = model.Duration;
            lastBookingHistory.TimeOut = DateTime.Now.AddDays(model.Duration);
            
            _dbContext.BookingHistoryTbl.Update(lastBookingHistory);
            var result = await _dbContext.SaveChangesAsync();
            if (result < 1)
            {
                return StatusCode(500, "InternalServerError");
            }

            return Ok(new { RoomDetails = room, Message = $"You have successfully updated the booking of room {room.RoomNumber}" });
        }


        /// <summary>
        /// This endpoint cancels a booking.
        /// It can only be accessed by a logged-in-user
        /// </summary>
        /// <param name="roomNumber">The room number of the booked room to be cancelled</param>
        /// <param name="AppUserId">This is the id of the currently-logged-in-user</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /api/appuser/remove-booking/roomNumber
        ///
        /// </remarks>
        /// <response code="200">If the booking is successfully cancelled</response>
        /// <response code="401">If the currently logged in user's id is different from the id passed in from the URL</response>
        /// <response code="400">If the roomm number provided does not match any booked room's number</response>
        /// <response code="500">If no change was made to the database</response>

        [HttpDelete("remove-booking/{roomNumber}", Name = nameof(RemoveBookingById))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> RemoveBookingById(byte roomNumber, string AppUserId)
        {
            // check if current logged-in user is the one sending the request
            AppUser currentLoggedInUser = await _userManager.GetUserAsync(User);
            if (currentLoggedInUser.Id != AppUserId)
            {
                ModelState.AddModelError("User", "Unauthorized");
                return Unauthorized(ModelState);
            }

            // delete booking
            BookingHistory bookedRoom = await _dbContext.BookingHistoryTbl.FirstOrDefaultAsync(x => x.RoomNumber == roomNumber);
            if (bookedRoom == null)
            {
                ModelState.AddModelError("Booked Room", $"Room {roomNumber} has not been booked");
                return BadRequest(ModelState);
            }
            _dbContext.Remove(bookedRoom);

            // set availability to 1 (room table)
            Room room = await _dbContext.RoomTbl.FirstOrDefaultAsync(x => x.RoomNumber == roomNumber);
            room.Availability = true;

            var result = await _dbContext.SaveChangesAsync();
            if (result < 1)
            {
                return StatusCode(500, "InternalServerError");
            }

            return Ok($"You have successfully unbooked room {roomNumber}");

        }

        /// <summary>
        /// This endpoint updates the profile of a user who is a Guest.
        /// It can only be accessed by a logged-in-user and the user must be a Guest
        /// </summary>
        /// <param name="model">This contains the user's new information</param>
        /// <param name="id">This is the id of the currently-logged-in-user</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /api/appuser/update-guest/id
        ///
        /// </remarks>
        /// <response code="200">If the profile was successfully updated</response>
        /// <response code="400">If no id was passed from the URL OR if the user does not exist in the database</response>
        /// <response code="401">If the user is not a Guest</response>
        /// <response code="500">If the system could not update the user's profile</response>

        [HttpPut("update-guest/{id}", Name = nameof(UpdateGuest))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> UpdateGuest([FromBody] UpdateAppUserDto model, string id)
        {
            var currentLoggedInUser = await _userManager.GetUserAsync(User);


            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("Id", "No id inputted");
                return BadRequest(ModelState);
            }

            if (currentLoggedInUser.Id != id)
            {
                ModelState.AddModelError("Id", "Do not have authorization access");
                return Unauthorized(ModelState);
            }

            // check if user is a SuperAdmin
            var roles = await _userManager.GetRolesAsync(currentLoggedInUser);
            if (!roles.Contains("Guest"))
            {
                ModelState.AddModelError("Unauthorized", "You're not authorized to update this profile");
                return Unauthorized(ModelState);
            }

            //check if user exist
            var userFromDb = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userFromDb == null)
            {
                ModelState.AddModelError("Error", "User does not exist");
                return BadRequest(ModelState);
            }

            userFromDb = _mapper.Map(model, userFromDb);

            var userIsUpdated = await _userManager.UpdateAsync(userFromDb);

            if (!userIsUpdated.Succeeded)
            {
                foreach (var err in userIsUpdated.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return StatusCode(500, "Internal Server Error");
            }

            return Ok("Your profile has been successfully updated");
        }

        /// <summary>
        /// This endpoint updates the profile of a user who is an Employee.
        /// It can only be accessed by a logged-in-user and the user must be an Employee
        /// </summary>
        /// <param name="model">This contains the user's new information</param>
        /// <param name="id">This is the id of the currently-logged-in-user</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /api/appuser/update-employee/id
        ///
        /// </remarks>
        /// <response code="200">If the profile was successfully updated</response>
        /// <response code="400">If no id was passed from the URL OR if the user does not exist in the database</response>
        /// <response code="401">If the user is not an Employee</response>
        /// <response code="500">If the system could not update the user's profile</response>

        [HttpPut("update-employee/{id}", Name = nameof(UpdateEmployee))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateAppUserDto model, string id)
        {
            var currentLoggedInUser = await _userManager.GetUserAsync(User);


            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("Id", "No id inputted");
                return BadRequest(ModelState);
            }

            if (currentLoggedInUser.Id != id)
            {
                ModelState.AddModelError("Id", "Do not have authorization access");
                return Unauthorized(ModelState);
            }

            // check if user is an Employee
            var roles = await _userManager.GetRolesAsync(currentLoggedInUser);
            if (!roles.Contains("Employee"))
            {
                ModelState.AddModelError("Unauthorized", "You're not authorized to update this profile");
                return Unauthorized(ModelState);
            }

            //check if user exist
            var userFromDb = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userFromDb == null)
            {
                ModelState.AddModelError("Error", "User does not exist");
                return BadRequest(ModelState);
            }

            userFromDb = _mapper.Map(model, userFromDb);

            var userIsUpdated = await _userManager.UpdateAsync(userFromDb);

            if (!userIsUpdated.Succeeded)
            {
                foreach (var err in userIsUpdated.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return StatusCode(500, "Internal Server Error");
            }

            return Ok("Your profile has been successfully updated");
        }

        /// <summary>
        /// This endpoint updates the profile of a user who is a SuperAdmin.
        /// It can only be accessed by a logged-in-user and the user must be an Employee
        /// </summary>
        /// <param name="model">This contains the user's new information</param>
        /// <param name="id">This is the id of the currently-logged-in-user</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /api/appuser/update-superadmin/id
        ///
        /// </remarks>
        /// <response code="200">If the profile was successfully updated</response>
        /// <response code="400">If no id was passed from the URL OR if the user does not exist in the database</response>
        /// <response code="401">If the user is not a SuperAdmin</response>
        /// <response code="500">If the system could not update the user's profile</response>

        [HttpPut("update-superadmin/{id}", Name = nameof(UpdateSuperAdmin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateSuperAdmin([FromBody] UpdateAppUserDto model, string id)
        {
            var currentLoggedInUser = await _userManager.GetUserAsync(User);


            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("Id", "No id inputted");
                return BadRequest(ModelState);
            }

            if (currentLoggedInUser.Id != id)
            {
                ModelState.AddModelError("Id", "Do not have authorization access");
                return Unauthorized(ModelState);
            }

            // check if user is a SuperAdmin
            var roles = await _userManager.GetRolesAsync(currentLoggedInUser);
            if (!roles.Contains("SuperAdmin"))
            {
                ModelState.AddModelError("Unauthorized", "You're not authorized to update this profile");
                return Unauthorized(ModelState);
            }

            //check if user exist
            var userFromDb = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userFromDb == null)
            {
                ModelState.AddModelError("Error", "User does not exist");
                return BadRequest(ModelState);
            }

            userFromDb = _mapper.Map(model, userFromDb);

            var userIsUpdated = await _userManager.UpdateAsync(userFromDb);

            if (!userIsUpdated.Succeeded)
            {
                foreach (var err in userIsUpdated.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return StatusCode(500, "Internal Server Error");
            }

            return Ok("Your profile has been successfully updated");
        }
    }
}
