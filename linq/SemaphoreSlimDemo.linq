<Query Kind="Program">
  <NuGetReference>ServiceStack.HttpClient.Core</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

using System.Threading.Tasks;
//SemaphoreSlim gate= new SemaphoreSlim(10);
//async Task Main()
//{
//	for (int i = 0; i < 10; i++)
//	{
//		"Start".Dump();
//		await gate.WaitAsync();
//		"Do some work".Dump();
//		"Finish".Dump();
//	}
//}


//SemaphoreSlim gate = new SemaphoreSlim(1);
//async Task Main()
//{
//	for (int i = 0; i < 10; i++)
//	{
//		"Start".Dump();
//		await gate.WaitAsync();
//		"Do some work".Dump();
//		gate.Release();
//		"Finish".Dump();
//	}
//}


HttpClient _client = new HttpClient();
SemaphoreSlim _gate = new SemaphoreSlim(10);

void Main() {
	Task.WaitAll(CreateCalls().ToArray());
	
}

public IEnumerable<Task> CreateCalls(){
	for (int i = 0; i < 100; i++)
	{
		yield return CallGoogle();
	}
}


public async Task CallGoogle(){
	try
	{
		await _gate.WaitAsync();
		var response = await _client.GetAsync("https://google.com");
		response.StatusCode.Dump();
		_gate.Release();
	}catch(Exception e){
		e.Message.Dump();
	}
	
}