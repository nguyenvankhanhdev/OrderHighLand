using Neo4j.Driver;
using OrderHighLand.Models;
using Category = OrderHighLand.Models.Category;

namespace OrderHighLand.Service
{
    public class ProductService
    {
        private readonly IDriver _driver;
        public ProductService(IDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }



        public async Task<List<Products>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            var result = await session.ExecuteReadAsync(async transaction =>
            {
                var readQuery = "MATCH (p:Product) RETURN p";
                var readResult = await transaction.RunAsync(readQuery);
                var products = await readResult.ToListAsync(record =>
                {
                    var node = record["p"].As<INode>();
                    return new Products
                    {
                        Id = node.Properties["PRO_ID"].As<int>(),
                        Name = node.Properties["PRO_NAME"].As<string>(),
                        Slug = node.Properties["PRO_SLUG"].As<string>(),
                        Cate_Id = node.Properties["CATE_ID"].As<int>()
                    };
                });
                return products;
            });
            await session.CloseAsync();
            return result;
        }

        private int getIdMax()
        {
            var session = _driver.AsyncSession();
            var result = session.ExecuteReadAsync(async transaction =>
            {
                var readQuery = "MATCH (p:Product) RETURN max(p.PRO_ID) as max";
                var readResult = await transaction.RunAsync(readQuery);
                var products = await readResult.ToListAsync(record =>
                {
                    var node = record["max"].As<int>();
                    return node;
                });
                return products;
            });
            session.CloseAsync();
            return result.Result[0];
        }
        public async Task<Products> CreateAsync(Products product)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteWriteAsync(async transaction =>
                {
                    // Truy vấn tạo node Product với các thuộc tính được truyền vào
                    var createQuery = @"
					CREATE (p:Product {
                    PRO_ID: $PRO_ID, 
                    PRO_NAME: $PRO_NAME, 
                    PRO_SLUG: $PRO_SLUG, 
                    PRO_IMAGE: $PRO_IMAGE, 
                    CATE_ID: $CATE_ID 
                })
                RETURN p";

                    // Tạo các tham số truy vấn
                    var createParams = new
                    {
                        PRO_ID = getIdMax() + 1, // Giả sử hàm getIdMax() trả về ID lớn nhất hiện có
                        PRO_NAME = product.Name,
                        PRO_SLUG = product.Slug,
                        PRO_IMAGE = product.Image,  // Assuming you have this property
                        CATE_ID = product.Cate_Id
                    };

                    // Thực thi truy vấn và lấy kết quả
                    var createResult = await transaction.RunAsync(createQuery, createParams);

                    // Xử lý kết quả trả về
                    var createdProduct = await createResult.SingleAsync(record =>
                    {
                        var node = record["p"].As<INode>();
                        return new Products
                        {
                            Id = node.Properties["PRO_ID"].As<int>(),
                            Name = node.Properties["PRO_NAME"].As<string>(),
                            Slug = node.Properties["PRO_SLUG"].As<string>(),
                            Image = node.Properties["PRO_IMAGE"].As<string>(),
                            Cate_Id = node.Properties["CATE_ID"].As<int>()
                        };
                    });

                    return createdProduct;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }



        public Task<T> DeleteAsync<T>(int id)
        {
            throw new NotImplementedException();
        }



        public Task<T> GetByIdAsync<T>(int id)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync<T>(int id, Products product)
        {
            throw new NotImplementedException();
        }
        //public async Task<Products> GetProductBySlug(string slug)
        //{
        //    var session = _driver.AsyncSession();
        //    try
        //    {
        //        var result = await session.ExecuteReadAsync(async transaction =>
        //        {
        //            // Sử dụng $slug để biểu thị biến được truyền vào từ mã C#
        //            var readQuery = "MATCH (p:Product {Slug: $slug}) RETURN p LIMIT 1";
        //            var cursor = await transaction.RunAsync(readQuery, new { slug });

        //            // Kiểm tra và lấy kết quả
        //            if (await cursor.FetchAsync())
        //            {
        //                var record = cursor.Current;
        //                var node = record["p"].As<INode>();

        //                return new Products
        //                {
        //                    Id = (int)node["Id"].As<long>(),
        //                    Name = node["Name"].As<string>(),
        //                    Slug = node["Slug"].As<string>(),
        //                    Image = node["Image"].As<string>(),
        //                    Cate_Id = (int)node["Cate_Id"].As<long>()
        //                };
        //            }
        //            return null;
        //        });
        //        return result;
        //    }
        //    finally
        //    {
        //        await session.CloseAsync();
        //    }
        //}

        public async Task<Products> GetProductBySlug(string slug)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async transaction =>
                {
                    var readQuery = @"
MATCH (p:Product {Slug: $slug})
OPTIONAL MATCH (p)<-[:VARIANT_OF]-(pv:ProductVariant)-[:HAS_SIZE]->(s:Size)
OPTIONAL MATCH (p)-[:BELONGS_TO]->(c:Category)
RETURN p, collect(pv) AS variants, collect(s) AS sizes, collect(c) AS categories LIMIT 1";


                    var cursor = await transaction.RunAsync(readQuery, new { slug });

                    if (await cursor.FetchAsync())
                    {
                        var record = cursor.Current;
                        var productNode = record["p"].As<INode>();
                        var variants = record["variants"].As<List<INode>>();
                        var sizes = record["sizes"].As<List<INode>>();
                        var categories = record["categories"].As<List<INode>>();

                        var product = new Products
                        {
                            Id = (int)productNode["Id"].As<long>(),
                            Name = productNode["Name"].As<string>(),
                            Slug = productNode["Slug"].As<string>(),
                            Image = productNode["Image"].As<string>(),
                            Cate_Id = (int)productNode["Cate_Id"].As<long>(),

                            ProductVariants = variants.Select(v => new ProductVariant
                            {
                                ID = (int)v["Id"].As<long>(),
                                PRO_ID = (int)v["Pro_Id"].As<long>(),
                                Price = v["Price"].As<float>(),
                                Quantity = (int)v["Quantity"].As<long>(),
                                S_ID = (int)v["Size_Id"].As<long>()
                            }).ToList(),

                            Sizes = sizes.Select(s => new Sizes
                            {
                                ID = (int)s["Id"].As<long>(),
                                S_SIZE = s["Size"].As<string>(),
                                S_PRICE = s["Price"].As<float>()
                            }).ToList(),

                            Categories = categories.Select(c => new Category
                            {
                                CATE_ID = (int)c["Id"].As<long>(),
                                CATE_NAME = c["Name"].As<string>(),
                                CATE_SLUG = c["Slug"].As<string>()
                            }).ToList()
                        };

                        return product;
                    }
                    return null;
                });
                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }


    }

}
