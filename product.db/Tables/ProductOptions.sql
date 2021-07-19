CREATE TABLE ProductOptions
(
      Id            uniqueidentifier    not null primary key
    , ProductId     uniqueidentifier    not null
    , Name          varchar(9)          not null
    , Description   varchar(23)             null
    , constraint fk_ProductOptions foreign key (ProductId) references Products (Id)
)