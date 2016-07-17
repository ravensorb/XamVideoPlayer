# XamVideoPlayer
Xamarin Cross Platform Video Player

[![Build status](https://ci.appveyor.com/api/projects/status/afrsbak8rflndj6b?svg=true)](https://ci.appveyor.com/project/ravensorb/xamvideoplayer)

[Get the package from nuget](https://www.nuget.org/packages/Xam.Plugins.VideoPlayer)

```
Install-Package Xam.Plugins.VideoPlayer -Pre 
```

## About

This library provides a fully implemented cross platform video player for Windows, iOS, and Android.  Please check out the samples to see how to use it.

Note: Remeber to take care of ATS for iOS Applications

```
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

Here is a sample XAML Page

```
<ContentPage x:Class="Xam.Plugins.VideoPlayer.Sample.Views.Page1"
             xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:videoPlayer="clr-namespace:Xam.Plugins.VideoPlayer;assembly=Xam.Plugins.VideoPlayer">
    <ContentPage.Resources>
        <ResourceDictionary>
            <videoPlayer:IntToTimeSpanConverter x:Key="intToTimeSpanConverter" />
        </ResourceDictionary>
    </ContentPage.Resources>
    <StackLayout Orientation="Vertical">
        <videoPlayer:VideoPlayer x:Name="video"
                                     BindingContext="{Binding SelectedVideo}"
                                     HeightRequest="300"
                                     VideoSource="{Binding PlaybackUrl}"
                                     WidthRequest="600" />
        <StackLayout Orientation="Horizontal">
            <Button Command="{Binding Path=SeekCommand, Source={x:Reference video}, Converter={StaticResource intToTimeSpanConverter}, ConverterParameter=-5}" Text="Rewind" />
            <Button Command="{Binding Path=PlayCommand, Source={x:Reference video}}" Text="Play" />
            <Button Command="{Binding Path=PauseCommand, Source={x:Reference video}}" Text="Pause" />
            <Button Command="{Binding Path=StopCommand, Source={x:Reference video}}" Text="Stop" />
            <Button Command="{Binding Path=SeekCommand, Source={x:Reference video}, Converter={StaticResource intToTimeSpanConverter}, ConverterParameter=5}" Text="FastForward" />
        </StackLayout>
        <StackLayout Orientation="Vertical">
            <Label Text="{Binding Position, StringFormat='Position {0}'}" />
        </StackLayout>
        <ListView x:Name="lstVideos"
                  ItemsSource="{Binding Videos}"
                  SelectedItem="{Binding SelectedVideo}">
            <ListView.ItemTemplate>
                <DataTemplate>
                    <ViewCell>
                        <ViewCell.View>
                            <StackLayout Orientation="Vertical">
                                <Label FontSize="Medium" Text="{Binding Title}" />
                            </StackLayout>
                        </ViewCell.View>
                    </ViewCell>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
    </StackLayout>
</ContentPage>
```
