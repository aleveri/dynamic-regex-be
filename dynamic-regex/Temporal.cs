using System.Resources;

namespace dynamic_regex
{
    public class CustomerPOValidationService : ICustomerPOValidationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerPOValidationMapper _validationMapper;

        public CustomerPOValidationService(IUnitOfWork unitOfWork,
            ICustomerPOValidationMapper validationMapper)
        {
            _unitOfWork = unitOfWork;
            _validationMapper = validationMapper;
        }

        public async Task<ResultDto<bool>> DeleteAsync(Guid id)
        {
            var validation = await _unitOfWork.CustomerPOValidationsRepository.GetByIdWithTrackingAsync(id) ?? throw new BizErrorException(string.Format(Resources.ResourceKeys.InvalidOperation, Resources.ResourceKeys.DeleteOperation, Resources.ResourceKeys.InstanceIdNotExists));
            var source = await _unitOfWork.CustomerPOValidationSourceAppRepository.GetByIdWithTrackingAsync(id);
            var category = await _unitOfWork.CustomerPOValidationCategoryRepository.GetByIdWithTrackingAsync(id);
            source.IsRowDeleted = true;
            category.IsRowDeleted = true;
            validation.IsRowDeleted = true;
            await _unitOfWork.CompleteAsync();
            return new ResultDto<bool>(true);
        }

        public async Task<ResultDto<CustomerPOValidationDto>> GetById(Guid id)
        {
            var validation = _unitOfWork.CustomerPOValidationsRepository.GetById(id);
            var sourceApp = _unitOfWork.CustomerPOValidationSourceAppRepository.GetByValidationId(id);
            var category = _unitOfWork.CustomerPOValidationCategoryRepository.GetByValidationId(id);
            var validationDto = _validationMapper.MapToDto(validation);
            validationDto.SourceAppName = sourceApp.Result.SourceAppName;
            validationDto.EditableSourceApp = sourceApp.Result.Editable;
            validationDto.SelectedSourceApp = sourceApp.Result.Selected;
            validationDto.NameValidationCategory = category.Result.Name;
            return new ResultDto<CustomerPOValidationDto>(validationDto);
        }

        public async Task<ResultDto<bool>> PostAsync(CustomerPOValidationDto request)
        {
            var exist = await _unitOfWork.CustomerPOValidationsRepository.ExistExpressionByCustomer(request.Expression, request.CustomerId);
            if (exist) return new ResultDto<bool>(false);
            var validation = _validationMapper.MapToEntity(request);
            await _unitOfWork.CustomerPOValidationsRepository.AddAsync(validation);
            await _unitOfWork.CompleteAsync();
            var sourceApp = new CustomerPOValidationSourceAppEntity
            {
                CustomerPoValidationId = validation.Id,
                SourceAppName = request.SourceAppName ?? string.Empty,
                Selected = request.SelectedSourceApp
            };
            var category = new CustomerPOValidationCategoryEntity()
            {
                CustomerPoValidationId = validation.Id,
                Name = request.NameValidationCategory ?? string.Empty
            };
            await _unitOfWork.CustomerPOValidationSourceAppRepository.AddAsync(sourceApp);
            await _unitOfWork.CustomerPOValidationCategoryRepository.AddAsync(category);
            await _unitOfWork.CompleteAsync();
            return new ResultDto<bool>(true);
        }

        public async Task<ResultDto<bool>> PutAsync(CustomerPOValidationDto request)
        {
            var validation = await _unitOfWork.CustomerPOValidationsRepository.GetByIdWithTrackingAsync(request.Id);
            var sourceApp = await _unitOfWork.CustomerPOValidationSourceAppRepository.GetByValidationId(request.Id);
            var category = await _unitOfWork.CustomerPOValidationCategoryRepository.GetByValidationId(request.Id);
            var validationEntity = _validationMapper.MapToEntity(request);
            var sourceAppNew = new CustomerPOValidationSourceAppEntity
            {
                CustomerPoValidationId = validation.Id,
                SourceAppName = request.SourceAppName ?? string.Empty,
                Selected = request.SelectedSourceApp
            };
            var categoryNew = new CustomerPOValidationCategoryEntity()
            {
                CustomerPoValidationId = validation.Id,
                Name = request.NameValidationCategory ?? string.Empty
            };
            _unitOfWork.CustomerPOValidationsRepository.Update(validationEntity, validation);
            _unitOfWork.CustomerPOValidationSourceAppRepository.Update(sourceAppNew, sourceApp);
            _unitOfWork.CustomerPOValidationCategoryRepository.Update(categoryNew, category);
            await _unitOfWork.CompleteAsync();
            return new ResultDto<bool>(true);
        }
    }
}
