 # WPF Extensions
 [![NuGet](https://img.shields.io/nuget/v/WpfEx.svg?style=for-the-badge)](https://www.nuget.org/packages/WpfEx)
 [![GitHub](https://img.shields.io/github/license/lezhkin11/wpf-extensions?style=for-the-badge)](https://github.com/lezhkin11/wpf-extensions/blob/master/LICENSE)

## Custom Grid Sorting

Without Custom Sorting     |  Custom Sorting
:-------------------------:|:-------------------------:
![no custom sorting](https://raw.githubusercontent.com/lezhkin11/wpf-extensions/master/docs/column_without_custom_sorting.png)  |  ![custom sorting](https://raw.githubusercontent.com/lezhkin11/wpf-extensions/master/docs/column_custom_sorting_str_logical_comparer.png)


## Usage 
1) Enable custom sorting for DataGrid by `UseCustomSort`
2) Set `CustomSorter` or `CustomSorterType` for columns you want apply custom sorting.
   - Sorter must implement `IComparer`
   - Sorter must have default constructor in order to use `CustomSorterType`

```xml
<DataGrid attached:DataGridHelpers.UseCustomSort="True" ItemsSource="{Binding Items}" AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn attached:DataGridHelpers.CustomSorterType="{x:Type comparers:StrLogicalComparer}" Binding="{Binding CodeText}" Header="Code"  />
        <DataGridTextColumn Header="Number" Binding="{Binding Number}" />
    </DataGrid.Columns>
</DataGrid>
```

## Supports Nested Properties

```xml
<DataGrid attached:DataGridHelpers.UseCustomSort="True" ItemsSource="{Binding Items}" AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn Binding="{Binding Item.Item1.Item2.TextNumber}" Header="Code" 
                            attached:DataGridHelpers.CustomSorterType="{x:Type comparers:StrLogicalComparer}" />
    </DataGrid.Columns>
</DataGrid>
```


## Extensions
- [Custom Sorter for the DataGrid Columns](https://github.com/lezhkin11/wpf-extensions/wiki/DataGrid-Columns-Custom-Sorter)

## Licence
[MIT License (MIT)](./LICENSE)
