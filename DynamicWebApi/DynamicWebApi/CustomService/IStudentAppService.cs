namespace DynamicWebApi.CustomService
{
    public interface IStudentAppService : IApplicationService
    {
        /// <summary>
        /// 根据ID获取学生
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StudentDto> Get(int id);

        /// <summary>
        /// 获取所有学生
        /// </summary>
        /// <returns></returns>
        Task<List<StudentDto>> Get();

        /// <summary>
        /// 更新学生信息
        /// </summary>
        /// <param name="input"></param>
        Task Update(Student input);

        /// <summary>
        /// 更新学生年龄
        /// </summary>
        /// <param name="age"></param>
        Task UpdateAge(int age);

        /// <summary>
        /// 根据ID删除学生
        /// </summary>
        /// <param name="id"></param>
        Task Delete(int id);

        /// <summary>
        /// 添加学生
        /// </summary>
        /// <param name="input"></param>
        Task Create(Student input);
    }
}
