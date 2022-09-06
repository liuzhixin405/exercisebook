
using PipelineFilter.Abstract;
using PipelineFilter.IService;
using PipelineFilter.Service;

IDataSource<Message> dataSource = new DataSource();
IDataSink<Message> dataSink = new DataSink();
PipelineBase<Message> pipeline = new Pipeline(dataSource,dataSink);

pipeline.Add(new AppendAFilter());
ActiveFilter activeFilter = new ActiveFilter();
pipeline.Add(activeFilter);
pipeline.Add(new AppendBFilter());
activeFilter.Action();

pipeline.Process();
Console.WriteLine($" data = {pipeline.Message.Data}");