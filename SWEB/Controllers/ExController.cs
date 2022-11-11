using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWEB.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ExController : Controller
    {
        static string currentPath = ".";
        // GET: ExController
        [HttpPost]
        public IActionResult FormatNumberRange([FromBody] FormatNumberRangeRequest request)
        {
            string result = string.Empty;
            bool skip = false;
            for(int i = 0; i < request.numbers.Length-1; i++)
            {
                if (request.numbers[i+1] - request.numbers[i] != 1)
                {
                    result += request.numbers[i];
                    result += ",";
                    skip = false;
                }
                else
                {
                    if (!skip)
                    {
                        result += request.numbers[i];
                        result += "-";
                        skip = true;
                    }
                }
            }
            result += request.numbers[request.numbers.Length-1];
            return new JsonResult(result);
        }

        [HttpGet]
        public IActionResult Excel(string coluumn)
        {
            int result = 0;
            for (int i = 0; i < coluumn.Length; i++)
            {
                result *= 26;
                result += coluumn[i] - 'A' + 1;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public IActionResult Pagination(PaginationRequest request)
        {
            int size = 0;
            int count = 1;
            int index = 0;
            PaginationResponse response;
            List<int> row = new List<int>();
            if (request.PageSize == 0)
            {
                return new JsonResult(new PaginationResponse { });
            }
            if (request.AllRows.Length % request.PageSize == 0)
            {
                size = request.PageSize;
            }
            else
            {
                size = request.PageSize+1;
            }
            if(request.PageIndex > size-1)
            {
                request.PageIndex = size-1;
            }
            for(int i = 0; i < size; i++)
            {
                if(i == request.PageIndex)
                {
                    index = i;
                    for (int j = (i)*request.PageSize;j< request.AllRows.Length; j++)
                    {
                        row.Add(request.AllRows[j]);
                        if (count == request.PageSize) break;
                        count++;
                    }
                }
                
            }
            
            response = new PaginationResponse { CurrentPageRows = row.ToArray(), From = ((index) * request.PageSize)+1, To = ((index) * request.PageSize) + 1 + (row.Count-1), Total = request.AllRows.Length };
            return new JsonResult(response);
        }

        [HttpPost]
        public IActionResult MatchingBraces([FromBody] string text)
        {
            bool result = false;
            Stack<char> left = new Stack<char>();
            Stack<char> right = new Stack<char>();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(' || text[i] == '[' || text[i] == '{')
                {
                    left.Push(text[i]);
                }
                else
                {
                    if (i == 0 || left.Count == 0 && i < text.Length)
                    {
                        return new JsonResult(result);
                    }                     
                    if (left.Peek() == '(' && text[i] == ')')
                    {
                        left.Pop();
                    }
                    else if (left.Peek() == '{' && text[i] == '}')
                    {
                        left.Pop();
                    }
                    else if (left.Peek() == '[' && text[i] == ']')
                    {
                        left.Pop();
                    }
                    else
                    {
                        return new JsonResult(result);
                    }

                }
                
            }
            if (left.Count == 0)
            {
                result = true;
            }
            
            return new JsonResult(result);
        }

        [HttpPost]
        public IActionResult URI(string text)
        {
            string Scheme = string.Empty;
            string UserInfo = string.Empty;
            string Host = string.Empty;
            string Port = string.Empty;
            string Path = string.Empty;
            string Query = string.Empty;
            string Fragment = string.Empty;

            
            for(int i = 0; i < 7; i++)
            {
                for(int current = 0; current < text.Length; current++)
                {
                    if(text[current] == ':' && text[current+1] == '/' && text[current+2] == '/')
                    {
                        Scheme = text.Remove(current);
                        text = text.Substring(current+3);
                        break;
                    }
                    if (text[current] == '@')
                    {
                        UserInfo = text.Remove(current);    
                        text = text.Substring(current+1);
                        break;
                    }
                    if (text[current] == '/' && Host.Equals(string.Empty))
                    {
                        
                        if (text.IndexOf(':') != -1)
                        {
                            Host = text.Remove(text.IndexOf(':'));
                            text = text.Substring(text.IndexOf(':') + 1);
                            for (int j = text.IndexOf(':')+1; j < text.Length; j++)
                            {
                                if (text[j] == '/')
                                {                                   
                                    Port = text.Remove(j);
                                    text = text.Substring(j+1);
                                    break;
                                }
                            }                           
                        }
                        else
                        {
                            Host = text.Remove(current);
                            text = text.Substring(current + 1);
                            break;
                        }
                        break;
                    }
                    Console.WriteLine("text->" + text);
                    Console.WriteLine("text current->" + text[current]);
                    if (text[current] == '?' && Path.Equals(string.Empty))
                    {
                        Console.WriteLine("current->Path->" + text.Remove(current));
                        Path = text.Remove(current);
                        text = text.Substring(current);
                        Console.WriteLine("local->Path->" + Path);
                        for (int j = 0; j < text.Length; j++)
                        {
                            if(text[j] == '#')
                            {
                                Query = text.Remove(j);
                                text = text.Substring(j);
                                break;
                            }
                            else if (j == text.Length-1)
                            {
                                Query = text;
                                text = "";
                                break;
                            }
                        }
                    }
                    else if (text[current] == '#')
                    {
                        if (Path.Equals(string.Empty))
                        {
                            Path = text.Remove(current);
                        }                      
                        Fragment = text;
                        text = "";
                        break;
                    }
                    if (current == text.Length - 1 && Host.Equals(string.Empty))
                    {
                        Host = text;
                        text = "";
                        break;
                    }
                    else if (current == text.Length - 1 && Path.Equals(string.Empty))
                    {
                        Path = text;
                        text = "";
                        break;
                    }

                }             
            }
            URIResponse result = new URIResponse { Scheme = Scheme, UserInfo = UserInfo, Host = Host, Port = Port, Path = Path, Query = Query, Fragment = Fragment};
            //return new JsonResult("-Scheme " + Scheme + " -UserInfo " + UserInfo + " -Host " + Host + " -Port " + Port + " -Path " + Path + " -Query " + Query + " -Fragment " + Fragment);
            return new JsonResult(result);
        }

        [HttpPost]
        public IActionResult Directory(string[] files)
        {
            string str = string.Empty;
            DirectoryResponse result = new DirectoryResponse();
            for(int i = 0; i < files.Length; i++)
            {
                result = AddtoDirectory(result, files[i], currentPath);
            }
            return new JsonResult(result);
        }


        class URIResponse
        {
            public string Scheme { get; set; } = string.Empty;
            public string UserInfo { get; set; } = string.Empty;
            public string Host { get; set; } = string.Empty;
            public string Port { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string Query { get; set; } = string.Empty;
            public string Fragment { get; set; } = string.Empty;
        }


        public class PaginationRequest
        {
            public int[] AllRows { get; set; } = new int[0];
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }

        public class PaginationResponse
        {
            public int[] CurrentPageRows { get; set; } = new int[0];
            public int From { get; set; }
            public int To { get; set; }
            public int Total { get; set; }
        }

        public class FormatNumberRangeRequest
        {
            public int[] numbers { get; set; }
        }

        class DirectoryResponse
        {
            public string Name { get; set; } = string.Empty;
            public List<DirectoryResponse> SubDirectories { get; set; } = new List<DirectoryResponse>();
            public List<string> Files { get; set; } = new List<string>();

            
        }

        static DirectoryResponse AddtoDirectory(DirectoryResponse ds, string files,string Path)
        {
            string str = string.Empty;
            DirectoryResponse result = new DirectoryResponse();
            for(int i = 0; i < files.Length; i++)
            {
                if (files[i] == '\\')
                {                  
                    if(ds.SubDirectories.Count > 0)
                    {
                        for (int j = 0; j < ds.SubDirectories.Count; j++)
                        {
                            if (ds.SubDirectories[j].Name.Equals(files.Remove(i)))
                            {
                                files = files.Substring(i + 1);
                                currentPath = ds.SubDirectories[j].Name;
                                AddtoDirectory(ds.SubDirectories[j], files, currentPath);
                                files = "";
                                return ds;
                            }
                        }
                        result.Name = files.Remove(i);
                        ds.SubDirectories.Add(AddtoDirectory(result, files.Substring(i + 1), currentPath));
                        files = "";
                        return ds;
                    }
                    else
                    {
                        result.Name = files.Remove(i);
                        ds.SubDirectories.Add(AddtoDirectory(result, files.Substring(i + 1), currentPath));
                        files = "";
                        return ds;
                    }
                    
                }
                else if(i == files.Length - 1)
                {
                    ds.Files.Add(files);
                    files = "";
                    return ds;
                }
            }
            return AddtoDirectory(ds,files, currentPath);
        }
    }
}
