using AutoMapper;
using TechYardHub.Core.DTO.AuthViewModel.FilesModel;
using TechYardHub.Core.Entity.Files;
using TechYardHub.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TechYardHub.Core.DTO;

namespace TechYardHub.Areas.Support.Controllers
{
    [Area("Support")]
    [Authorize(Policy = "Support Developer")]
    public class PathController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;
        private const string CacheKey = "pathsCache";
        public PathController(IUnitOfWork _unitOfWork, IMapper _mapper, IMemoryCache _memoryCache)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            memoryCache = _memoryCache;
        }
        // GET: PathController
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Paths";
                if (!memoryCache.TryGetValue(CacheKey, out IEnumerable<Paths>? AllPaths))
                {
                    AllPaths = await unitOfWork.PathsRepository.GetAllAsync();
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5), // Cache for 5 minutes
                        SlidingExpiration = TimeSpan.FromMinutes(2) // Reset cache if accessed within 2 minutes
                    };
                    memoryCache.Set(CacheKey, AllPaths, cacheEntryOptions);
                }
                return View(AllPaths);
            }
            catch (Exception ex)
            {
                var errorViewModel = new ErrorViewModel
                {
                    Message = "خطا في جلب البيانات",
                    StackTrace = ex.StackTrace
                };
                return View("~/Views/Shared/Error.cshtml", errorViewModel);
            }
        }

        // GET: PathController/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Create Path";
            return View();
        }

        // POST: PathController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PathsModel pathsModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check for duplicate Path Name
                    var existingPath = await unitOfWork.PathsRepository.GetFirstOrDefaultAsync(p => p.Name == pathsModel.Name);
                    if (existingPath != null)
                    {
                        ModelState.AddModelError("Name", "Path name already exists. Please choose a different name.");
                        return View(pathsModel);
                    }

                    var paths = mapper.Map<Paths>(pathsModel);
                    await unitOfWork.PathsRepository.AddAsync(paths);
                    await unitOfWork.SaveChangesAsync();
                    memoryCache.Remove(CacheKey); // Clear cache

                    return RedirectToAction(nameof(Index));
                }
                return View(pathsModel);
            }
            catch (Exception ex)
            {
                var errorViewModel = new ErrorViewModel
                {
                    Message = "Error saving data",
                    StackTrace = ex.StackTrace
                };
                return View("~/Views/Shared/Error.cshtml", errorViewModel);
            }
        }

        // GET: PathController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                ViewData["Title"] = "Edit Path";
                var Path = (memoryCache.TryGetValue(CacheKey, out IEnumerable<Paths>? Paths)) ?
                    Paths?.FirstOrDefault(s => s.Id == id) :
                    await unitOfWork.PathsRepository.GetByIdAsync(id);
                return View(Path);
            }
            catch (Exception ex)
            {
                var errorViewModel = new ErrorViewModel
                {
                    Message = "خطا في جلب البيانات",
                    StackTrace = ex.StackTrace
                };
                return View("~/Views/Shared/Error.cshtml", errorViewModel);
            }
        }

        // POST: PathController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PathsModel pathsModel, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var pathToUpdate = await unitOfWork.PathsRepository.GetByIdAsync(id);
                    if (pathToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Prevent duplicate Path Name (ignoring the current path)
                    var duplicatePath = await unitOfWork.PathsRepository.GetFirstOrDefaultAsync(p => p.Name == pathsModel.Name && p.Id != id);
                    if (duplicatePath != null)
                    {
                        ModelState.AddModelError("Name", "Path name already exists. Please choose a different name.");
                        return View(pathsModel);
                    }

                    // Map updated data to the existing path entity
                    pathToUpdate.Name = pathsModel.Name;
                    pathToUpdate.Description = pathsModel.Description;

                    unitOfWork.PathsRepository.Update(pathToUpdate);
                    await unitOfWork.SaveChangesAsync();
                    memoryCache.Remove(CacheKey); // Clear cache

                    ViewBag.Message = "Data updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                return View(pathsModel);
            }
            catch (Exception ex)
            {
                var errorViewModel = new ErrorViewModel
                {
                    Message = "Error updating data",
                    StackTrace = ex.StackTrace
                };
                return View("~/Views/Shared/Error.cshtml", errorViewModel);
            }
        }

        // POST: PathController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var path = await unitOfWork.PathsRepository.GetByIdAsync(id);
                if (path == null)
                {
                    return NotFound();
                }

                unitOfWork.PathsRepository.Delete(path);
                await unitOfWork.SaveChangesAsync();
                memoryCache.Remove(CacheKey); // Clear cache

                ViewBag.Message = "Data deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var errorViewModel = new ErrorViewModel
                {
                    Message = "Error deleting data",
                    StackTrace = ex.StackTrace
                };
                return View("~/Views/Shared/Error.cshtml", errorViewModel);
            }
        }
    }
}
