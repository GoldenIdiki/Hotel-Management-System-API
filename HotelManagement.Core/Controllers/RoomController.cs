using AutoMapper;
using HotelManagement.Data;
using HotelManagement.Data.Dtos;
using HotelManagement.Data.Models;
using HotelManagement.Data.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HotelManagement.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Guest,Employee,SuperAdmin")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly HotelManagementDbContext _dbContext;

        public RoomController(IRoomRepository roomRepository, IMapper mapper, IFileUpload fileUpload, HotelManagementDbContext dbContext)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
            _fileUpload = fileUpload;
            _dbContext = dbContext;
        }

        /// <summary>
        /// This endpoint gets all the rooms in the hotel.
        /// It can only be accessed by a logged-in-user.
        /// </summary>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/room
        ///
        /// </remarks>
        /// <response code="200">If all the rooms were successfully gotten from the database</response>
        /// <response code="404">If no room exists in the database</response>
        
        [HttpGet(Name = nameof(GetAllRooms))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllRooms()
        {
            var roomsFromDb = await _roomRepository.GetRooms();
            if (!roomsFromDb.Any())
            {
                ModelState.AddModelError("Get Rooms", "No room in this hotel");
                return NotFound(ModelState);
            }
           
            var viewRooms = _mapper.Map<IEnumerable<RoomResponseDto>>(roomsFromDb);
            return Ok(viewRooms);
        }

        /// <summary>
        /// This endpoint gets a room by its id.
        /// It can only be accessed by a logged-in-user.
        /// </summary>
        /// <param name="id">This is the id of room to be gotten</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/room/get-room/id
        ///
        /// </remarks>
        /// <response code="200">If the required room was successfully gotten from the database</response>
        /// <response code="404">If no room has that id in the database</response>
        [HttpGet("get-room/{id}", Name = nameof(GetRoomById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomById(string id)
        {
            Room roomFromDb = await _roomRepository.GetRoomById(id);
            if (roomFromDb == null)
            {
                ModelState.AddModelError("Room", "Requested room not found");
                return NotFound(ModelState);
            }
            var viewRoom = _mapper.Map<RoomResponseDto>(roomFromDb);
            return Ok(viewRoom);
        }

        /// <summary>
        /// This endpoint gets a room by its room number.
        /// It can only be accessed by a logged-in-user.
        /// </summary>
        /// <param name="roomNumber">This is the room number of the room to be gotten</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/room/get-room/roomNumber
        ///
        /// </remarks>
        /// <response code="200">If the required room was successfully gotten from the database</response>
        /// <response code="404">If no room has that id in the database</response>
        [HttpGet("get-room/{roomNumber}", Name = nameof(GetRoomByRoomNumber))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomByRoomNumber(byte roomNumber)
        {
            Room roomFromDb = await _roomRepository.GetRoomByRoomNumber(roomNumber);
            if (roomFromDb == null)
            {
                ModelState.AddModelError("Room", "Requested room not found");
                return NotFound(ModelState);
            }
            var viewRoom = _mapper.Map<RoomResponseDto>(roomFromDb);
            return Ok(viewRoom);
        }

        /// <summary>
        /// This endpoint gets all available rooms.
        /// It can only be accessed by a logged-in-user.
        /// </summary>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/room/get-available-rooms
        ///
        /// </remarks>
        /// <response code="200">If all the available rooms were successfully gotten from the database</response>
        /// <response code="404">If no room is available at the moment</response>
        [HttpGet("get-available-rooms", Name = nameof(GetAvailableRooms))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvailableRooms()
        {
            var roomsFromDb = await _roomRepository.GetAvailableRooms();
            if (!roomsFromDb.Any())
            {
                ModelState.AddModelError("Get Rooms", "There is no room available");
                return NotFound(ModelState);
            }
            var viewRooms = _mapper.Map<IEnumerable<RoomResponseDto>>(roomsFromDb);
            return Ok(viewRooms);
        }

        /// <summary>
        /// This endpoint gets all booked rooms.
        /// It can only be accessed by a logged-in-user who is either an Employee or a SuperAdmin.
        /// </summary>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/room/get-booked-rooms
        ///
        /// </remarks>
        /// <response code="200">If all the booked rooms were successfully gotten from the database</response>
        /// <response code="404">If no room is currently booked</response>
        [HttpGet("get-booked-rooms", Name = nameof(GetBookedRooms))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Employee,SuperAdmin")]
        public async Task<IActionResult> GetBookedRooms()
        {
            var roomsFromDb = await _roomRepository.GetBookedRooms();
            if (!roomsFromDb.Any())
            {
                ModelState.AddModelError("Booked Rooms", "No room has been booked");
                return NotFound(ModelState);
            }
            var viewRooms = _mapper.Map<IEnumerable<RoomResponseDto>>(roomsFromDb);
            
            return Ok(viewRooms);
        }

        /// <summary>
        /// This endpoint checks if a room is available for booking.
        /// It can only be accessed by a logged-in-user.
        /// </summary>
        /// <param name="roomNumber">This is the room number of the room to be checked</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/room/room-availability/roomNumber
        ///
        /// </remarks>
        /// <response code="200">If the required room is available for booking</response>
        /// <response code="400">If the requested room does not exist in the database OR it is currently booked</response>
        [HttpGet("room-availability/{roomNumber}", Name = nameof(IsRoomAvailable))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> IsRoomAvailable(byte roomNumber)
        {
            // check if room exist in the database
            var roomFromDb = await _roomRepository.GetRoomByRoomNumber(roomNumber);
            if (roomFromDb == null)
            {
                ModelState.AddModelError("Room", "The requested room does not exist");
                return BadRequest(ModelState);
            }

            // check if the requested room is available for booking
            var roomCheck = await _roomRepository.IsRoomAvailable(roomNumber);
            if (roomCheck == false)
            {
                ModelState.AddModelError("Check Room", "Sorry, the requested room is not available");
                return BadRequest(ModelState);
            }

            Room room = await _roomRepository.GetRoomByRoomNumber(roomNumber);
            var viewRoom = _mapper.Map<RoomResponseDto>(room);
            return Ok(new { room_details = viewRoom, message = "Requested room is available for booking"});
        }

        /// <summary>
        /// This endpoint updates a room's picture.
        /// It can only be accessed by a logged-in-user who is either an Employee or a SuperAdmin.
        /// </summary>
        /// <param name="photo">This is the picture to be used for the room</param>
        /// <param name="id">This is the id of the room</param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/room/update-roomphoto/id
        ///
        /// </remarks>
        /// <response code="200">If the selected picture was successfully added to the room</response>
        /// <response code="400">If the selected picture could not be added to the room</response>
        /// <response code="404">If the room with the given id was not found in the database</response>
        [HttpPatch("update-roomphoto/{id}", Name = nameof(UpdateRoomPhoto))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Employee,SuperAdmin")]
        public async Task<IActionResult> UpdateRoomPhoto(IFormFile photo, string id)
        {
            var result = await _roomRepository.GetRoomById(id);
            if (result == null)
            {
                ModelState.AddModelError("Empty Room", "Requsted room does not exist");
                return NotFound(ModelState);
            }
            var photoInfo = _fileUpload.UploadAvatar(photo);
            result.RoomPhotoUrl = photoInfo.AvatarUrl;

            var isUpdated = await _roomRepository.UpdateRoomPhoto(result);
            if (!isUpdated)
            {
                ModelState.AddModelError("", "Photo not updated");
                return BadRequest(ModelState);
            }
            var response = new UploadAvatarResponseDto()
            {
                AvatarUrl = result.RoomPhotoUrl
            };

            return Ok(response);
        }


        /// <summary>
        /// This endpoint makes all rooms available.
        /// It can only be accessed by a logged-in-user who is either an Employee or a SuperAdmin.
        /// </summary>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/room/make-rooms-available
        ///
        /// </remarks>
        /// <response code="200">If all the overdue rooms are now available for booking</response>
        /// <response code="400">If no room is overdue at the moment OR if the database could be not be updated</response>
        [HttpPatch("make-rooms-available", Name = nameof(MakeAllOverDueRoomsAvailable))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Employee,SuperAdmin")]
        public async Task<IActionResult> MakeAllOverDueRoomsAvailable()
        {
            // check if any room is overdue
            var overDueRooms = await _roomRepository.GetOverdueRooms();
            if (!overDueRooms.Any())
            {
                ModelState.AddModelError("OverdueBooked Rooms", "No room is overdue");
                return BadRequest(ModelState);
            }

            // make all rooms available
            foreach (Room room in overDueRooms)
            {
                room.Availability = true;
            }

            // update database
            _dbContext.RoomTbl.UpdateRange(overDueRooms);
            int result = await _dbContext.SaveChangesAsync();
            if (result < 1)
            {
                ModelState.AddModelError("Overdue Rooms", "Something went wrong");
                return BadRequest(ModelState);
            }

            return Ok("All rooms are now available");
        }

        /// <summary>
        /// This endpoint makes a particular room available.
        /// It can only be accessed by a logged-in-user who is either an Employee or a SuperAdmin.
        /// </summary>
        /// <param name="roomId"></param>

        /// <returns>This endpoint returns a status code</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/room/make-room-available/roomId
        ///
        /// </remarks>
        /// <response code="200">If the required overdue room is now available for booking</response>
        /// <response code="400">If no room is overdue at the moment OR if the database could be not be updated OR if the required room does not exist OR if the required room is not overdue</response>
        [HttpPatch("make-room-available/{roomId}", Name = nameof(MakeAnOverDueRoomAvailableByRoomId))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Employee,SuperAdmin")]
        public async Task<IActionResult> MakeAnOverDueRoomAvailableByRoomId([FromQuery] string roomId)
        {
            // check if any room is overdue
            var overDueRooms = await _roomRepository.GetOverdueRooms();
            if (!overDueRooms.Any())
            {
                ModelState.AddModelError("OverdueBooked Rooms", "No room is overdue");
                return BadRequest(ModelState);
            }

            // check if any room in the database has the inputted id
            var room = await _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                ModelState.AddModelError("NotFound", "Required room does not exist");
                return BadRequest(ModelState);
            }

            // check if the required room is overdue
            if (!overDueRooms.Contains(room))
            {
                ModelState.AddModelError("Room", $"Room {room.RoomNumber} is not yet overdue");
                return BadRequest(ModelState);
            }

            room.Availability = true;

            // update database
            _dbContext.RoomTbl.Update(room);
            int result = await _dbContext.SaveChangesAsync();
            if (result < 1)
            {
                ModelState.AddModelError("Overdue Room", "Something went wrong");
                return BadRequest(ModelState);
            }

            return Ok($"room {room.RoomNumber} is now available for booking");
        }
    }
}
