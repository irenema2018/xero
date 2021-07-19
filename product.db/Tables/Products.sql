create table Products
(
      Id            uniqueidentifier    not null primary key 
    , Name          varchar(17)         not null
    , Description   varchar(35)             null
    , Price         decimal(6, 2)       not null
    , DeliveryPrice decimal(4, 2)       not null
)
