using Microsoft.AspNetCore.Mvc;

namespace DynamicWebApi.CustomService
{
    public class StudentAppService : IStudentAppService
    {
        /// <summary>
        /// 根据ID获取学生
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public Task<StudentDto> Get(int id)
        {
            
            return Task.FromResult( new StudentDto() { Id = 1, Age = 18, Name = "张三" });
        }

        /// <summary>
        /// 获取所有学生
        /// </summary>
        /// <returns></returns>
        public Task<List<StudentDto>> Get()
        {
            return Task.FromResult( new List<StudentDto>()
        {
            new StudentDto(){Id = 1,Age = 18,Name = "张三"},
            new StudentDto(){Id = 2,Age = 19,Name = "李四"}
        });
        }

        /// <summary>
        /// 更新学生信息
        /// </summary>
        /// <param name="input"></param>
        public Task Update(Student input)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 更新学生年龄
        /// </summary>
        /// <param name="age"></param>
        [HttpPatch("{id:int}/age")]
        public Task UpdateAge(int age)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 根据ID删除学生
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id:int}")]
        public Task Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 添加学生
        /// </summary>
        /// <param name="input"></param>
        public Task Create(Student input)
        {
            throw new System.NotImplementedException();
        }
    }
}
