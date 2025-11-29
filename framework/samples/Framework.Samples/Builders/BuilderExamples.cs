namespace Framework.Samples.Builders;

/// <summary>
/// 电脑产品
/// </summary>
public class Computer
{
    public string CPU { get; set; } = string.Empty;
    public string RAM { get; set; } = string.Empty;
    public string Storage { get; set; } = string.Empty;
    public string GPU { get; set; } = string.Empty;
    public string Motherboard { get; set; } = string.Empty;
    public string PowerSupply { get; set; } = string.Empty;
    public List<string> Accessories { get; set; } = new();

    public void Display()
    {
        Console.WriteLine("[建造者示例] 电脑配置:");
        Console.WriteLine($"  CPU: {CPU}");
        Console.WriteLine($"  内存: {RAM}");
        Console.WriteLine($"  存储: {Storage}");
        Console.WriteLine($"  显卡: {GPU}");
        Console.WriteLine($"  主板: {Motherboard}");
        Console.WriteLine($"  电源: {PowerSupply}");
        if (Accessories.Any())
        {
            Console.WriteLine($"  配件: {string.Join(", ", Accessories)}");
        }
    }
}

/// <summary>
/// 电脑建造者接口
/// </summary>
public interface IComputerBuilder
{
    IComputerBuilder SetCPU(string cpu);
    IComputerBuilder SetRAM(string ram);
    IComputerBuilder SetStorage(string storage);
    IComputerBuilder SetGPU(string gpu);
    IComputerBuilder SetMotherboard(string motherboard);
    IComputerBuilder SetPowerSupply(string powerSupply);
    IComputerBuilder AddAccessory(string accessory);
    Computer Build();
}

/// <summary>
/// 具体的电脑建造者
/// </summary>
public class ComputerBuilder : IComputerBuilder
{
    private readonly Computer _computer = new Computer();

    public IComputerBuilder SetCPU(string cpu)
    {
        _computer.CPU = cpu;
        Console.WriteLine($"[建造者示例] 安装CPU: {cpu}");
        return this;
    }

    public IComputerBuilder SetRAM(string ram)
    {
        _computer.RAM = ram;
        Console.WriteLine($"[建造者示例] 安装内存: {ram}");
        return this;
    }

    public IComputerBuilder SetStorage(string storage)
    {
        _computer.Storage = storage;
        Console.WriteLine($"[建造者示例] 安装存储: {storage}");
        return this;
    }

    public IComputerBuilder SetGPU(string gpu)
    {
        _computer.GPU = gpu;
        Console.WriteLine($"[建造者示例] 安装显卡: {gpu}");
        return this;
    }

    public IComputerBuilder SetMotherboard(string motherboard)
    {
        _computer.Motherboard = motherboard;
        Console.WriteLine($"[建造者示例] 安装主板: {motherboard}");
        return this;
    }

    public IComputerBuilder SetPowerSupply(string powerSupply)
    {
        _computer.PowerSupply = powerSupply;
        Console.WriteLine($"[建造者示例] 安装电源: {powerSupply}");
        return this;
    }

    public IComputerBuilder AddAccessory(string accessory)
    {
        _computer.Accessories.Add(accessory);
        Console.WriteLine($"[建造者示例] 添加配件: {accessory}");
        return this;
    }

    public Computer Build()
    {
        Console.WriteLine("[建造者示例] 电脑组装完成！");
        return _computer;
    }
}

/// <summary>
/// 指挥者 - 封装预定义的构建流程
/// </summary>
public class ComputerDirector
{
    public Computer BuildGamingComputer(IComputerBuilder builder)
    {
        Console.WriteLine("[建造者示例] 开始构建游戏电脑...");
        return builder
            .SetCPU("Intel Core i9-13900K")
            .SetRAM("32GB DDR5")
            .SetStorage("2TB NVMe SSD")
            .SetGPU("NVIDIA RTX 4090")
            .SetMotherboard("ASUS ROG MAXIMUS Z790")
            .SetPowerSupply("1000W 80+ Gold")
            .AddAccessory("RGB灯条")
            .AddAccessory("水冷散热器")
            .Build();
    }

    public Computer BuildOfficeComputer(IComputerBuilder builder)
    {
        Console.WriteLine("[建造者示例] 开始构建办公电脑...");
        return builder
            .SetCPU("Intel Core i5-13400")
            .SetRAM("16GB DDR4")
            .SetStorage("512GB SSD")
            .SetGPU("集成显卡")
            .SetMotherboard("MSI B760M")
            .SetPowerSupply("500W 80+ Bronze")
            .Build();
    }

    public Computer BuildWorkstationComputer(IComputerBuilder builder)
    {
        Console.WriteLine("[建造者示例] 开始构建工作站...");
        return builder
            .SetCPU("AMD Ryzen Threadripper PRO")
            .SetRAM("128GB ECC DDR4")
            .SetStorage("4TB NVMe SSD")
            .SetGPU("NVIDIA RTX A6000")
            .SetMotherboard("ASUS Pro WS WRX80E")
            .SetPowerSupply("1200W 80+ Platinum")
            .AddAccessory("专业音频卡")
            .AddAccessory("多屏显示器支架")
            .Build();
    }
}

/// <summary>
/// HTTP请求 - 建造者模式示例
/// </summary>
public class HttpRequest
{
    public string Method { get; set; } = "GET";
    public string Url { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public Dictionary<string, string> QueryParameters { get; set; } = new();
    public string? Body { get; set; }
    public int Timeout { get; set; } = 30;

    public void Display()
    {
        Console.WriteLine("[建造者示例] HTTP请求:");
        Console.WriteLine($"  方法: {Method}");
        Console.WriteLine($"  URL: {Url}");
        Console.WriteLine($"  超时: {Timeout}秒");
        if (Headers.Any())
        {
            Console.WriteLine("  请求头:");
            foreach (var header in Headers)
            {
                Console.WriteLine($"    {header.Key}: {header.Value}");
            }
        }
        if (QueryParameters.Any())
        {
            Console.WriteLine("  查询参数:");
            foreach (var param in QueryParameters)
            {
                Console.WriteLine($"    {param.Key}={param.Value}");
            }
        }
        if (!string.IsNullOrEmpty(Body))
        {
            Console.WriteLine($"  请求体: {Body}");
        }
    }
}

/// <summary>
/// HTTP请求建造者
/// </summary>
public class HttpRequestBuilder
{
    private readonly HttpRequest _request = new HttpRequest();

    public HttpRequestBuilder SetMethod(string method)
    {
        _request.Method = method;
        return this;
    }

    public HttpRequestBuilder SetUrl(string url)
    {
        _request.Url = url;
        return this;
    }

    public HttpRequestBuilder AddHeader(string key, string value)
    {
        _request.Headers[key] = value;
        return this;
    }

    public HttpRequestBuilder AddQueryParameter(string key, string value)
    {
        _request.QueryParameters[key] = value;
        return this;
    }

    public HttpRequestBuilder SetBody(string body)
    {
        _request.Body = body;
        return this;
    }

    public HttpRequestBuilder SetTimeout(int seconds)
    {
        _request.Timeout = seconds;
        return this;
    }

    public HttpRequest Build()
    {
        if (string.IsNullOrEmpty(_request.Url))
        {
            throw new InvalidOperationException("URL不能为空");
        }
        return _request;
    }

    // 便捷方法
    public static HttpRequestBuilder Get(string url) =>
        new HttpRequestBuilder().SetMethod("GET").SetUrl(url);

    public static HttpRequestBuilder Post(string url) =>
        new HttpRequestBuilder().SetMethod("POST").SetUrl(url);

    public static HttpRequestBuilder Put(string url) =>
        new HttpRequestBuilder().SetMethod("PUT").SetUrl(url);

    public static HttpRequestBuilder Delete(string url) =>
        new HttpRequestBuilder().SetMethod("DELETE").SetUrl(url);
}
