using System.Reflection;

namespace XCEngine.Core
{
    public static class MethodInfoExtension
    {
        /// <summary>
        /// 执行异步Method方法
        /// </summary>
        /// <param name="this"></param>
        /// <param name="obj"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            Task task = (Task)@this.Invoke(obj, parameters);
            await task;
            PropertyInfo resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }
    }
}
