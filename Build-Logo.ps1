[CmdletBinding()]
param(
    [ValidateSet('All', 'Icon', 'SocialMedia')]
    [string] $Target = 'All'
)

$ErrorActionPreference = 'Stop'

if (-not (Get-Command magick -ErrorAction SilentlyContinue)) {
    throw 'ImageMagick is required. Install it with winget install ImageMagick.ImageMagick.'
}

$sourceDirectory = Join-Path $PSScriptRoot 'assets'
$socialMediaSourceDirectory = Join-Path $sourceDirectory 'social-media'
$temporaryPath = Join-Path $sourceDirectory ('.branding-' + [System.Guid]::NewGuid().ToString('N'))
$iconSizes = 512, 256, 128, 64, 48, 32, 16

$pageBackgroundColor = '#F4F1E8'
$backgroundColor = '#172033'
# Preblended equivalent of #0F1626 at 70% over the background. The source
# background has a shadow-shaped knockout, so the generated facet must be opaque.
$shadowColor = '#11192A'
$hourglassColor = '#D6A84A'
$bodyTextColor = '#475569'
$secondaryTealColor = '#286B68'

function Invoke-ImageMagick {
    param([Parameter(Mandatory)][string[]] $Arguments)

    & magick @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "ImageMagick failed with exit code $LASTEXITCODE."
    }
}

function New-ColoredLayer {
    param(
        [Parameter(Mandatory)][string] $Source,
        [Parameter(Mandatory)][string] $Color,
        [Parameter(Mandatory)][string] $Destination
    )

    Invoke-ImageMagick @(
        $Source,
        '-channel', 'RGB',
        '-fill', $Color,
        '-colorize', '100',
        '+channel',
        $Destination
    )
}

function New-Composition {
    param(
        [Parameter(Mandatory)][string[]] $Layers,
        [Parameter(Mandatory)][string] $Destination
    )

    $flattened = Join-Path $temporaryPath 'icon-flattened.png'
    $trimmed = Join-Path $temporaryPath 'icon-trimmed.png'

    Invoke-ImageMagick @(
        $Layers
        '-background', 'none'
        '-layers', 'flatten'
        $flattened
    )
    Invoke-ImageMagick @($flattened, '-trim', '+repage', $trimmed)

    $dimensions = (& magick identify -format '%w %h' $trimmed) -split ' '
    if ($LASTEXITCODE -ne 0 -or $dimensions.Count -ne 2) {
        throw 'ImageMagick could not determine the composition dimensions.'
    }

    $contentSize = [Math]::Max([int] $dimensions[0], [int] $dimensions[1])
    $padding = [Math]::Ceiling($contentSize * 0.075)
    $canvasSize = $contentSize + (2 * $padding)

    Invoke-ImageMagick @(
        $trimmed,
        '-background', 'none',
        '-bordercolor', 'none',
        '-border', "${padding}x${padding}",
        '-gravity', 'center',
        '-extent', "${canvasSize}x${canvasSize}",
        $Destination
    )
}

function New-IconComposition {
    param([Parameter(Mandatory)][string] $Destination)

    $background = Join-Path $temporaryPath 'icon-background.png'
    $shadow = Join-Path $temporaryPath 'icon-shadow.png'
    $hourglassLeft = Join-Path $temporaryPath 'icon-hourglass-left.png'
    $hourglassMiddle = Join-Path $temporaryPath 'icon-hourglass-middle.png'
    $hourglassRight = Join-Path $temporaryPath 'icon-hourglass-right.png'

    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-light.png') `
        -Color $backgroundColor `
        -Destination $background
    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-shadow.png') `
        -Color $shadowColor `
        -Destination $shadow
    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-left.png') `
        -Color $hourglassColor `
        -Destination $hourglassLeft
    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-middle.png') `
        -Color $backgroundColor `
        -Destination $hourglassMiddle
    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-right.png') `
        -Color $hourglassColor `
        -Destination $hourglassRight

    New-Composition `
        -Layers @($background, $shadow, $hourglassMiddle, $hourglassLeft, $hourglassRight) `
        -Destination $Destination
}

function New-ResizedPng {
    param(
        [Parameter(Mandatory)][string] $Source,
        [Parameter(Mandatory)][int] $Size,
        [Parameter(Mandatory)][string] $Destination
    )

    Invoke-ImageMagick @(
        $Source,
        '-filter', 'Lanczos',
        '-resize', "${Size}x${Size}",
        '-strip',
        '-define', 'png:exclude-chunk=date,time',
        $Destination
    )
}

function New-IconAssets {
    param([Parameter(Mandatory)][string] $Composition)

    $resized = @{}
    foreach ($size in $iconSizes) {
        $destination = if ($size -in 512, 256, 128, 64) {
            Join-Path $sourceDirectory "chrononuensis-icon-$size.png"
        }
        else {
            Join-Path $temporaryPath "chrononuensis-icon-$size.png"
        }

        New-ResizedPng -Source $Composition -Size $size -Destination $destination
        $resized[$size] = $destination
    }

    Copy-Item -LiteralPath $resized[128] -Destination (Join-Path $PSScriptRoot 'chrononuensis-icon-128.png') -Force
    Copy-Item -LiteralPath $resized[64] -Destination (Join-Path $sourceDirectory 'chrononuensis-favicon.png') -Force
    Copy-Item -LiteralPath $resized[64] -Destination (Join-Path $PSScriptRoot 'docs/uploads/chrononuensis-icon-64.png') -Force
    Copy-Item -LiteralPath $resized[256] -Destination (Join-Path $PSScriptRoot 'docs/uploads/chrononuensis-icon-256.png') -Force
    Copy-Item -LiteralPath $resized[64] -Destination (Join-Path $PSScriptRoot 'docs/uploads/favicon.png') -Force

    $iconDestination = Join-Path $sourceDirectory 'chrononuensis.ico'
    Invoke-ImageMagick @(
        $resized[256],
        $resized[128],
        $resized[64],
        $resized[48],
        $resized[32],
        $resized[16],
        $iconDestination
    )

    Copy-Item -LiteralPath $iconDestination -Destination (Join-Path $sourceDirectory 'favicon.ico') -Force
    Copy-Item -LiteralPath $iconDestination -Destination (Join-Path $PSScriptRoot 'docs/uploads/favicon.ico') -Force
}

function New-SocialMediaBanner {
    param([Parameter(Mandatory)][string] $IconComposition)

    $layerSources = [ordered]@{
        Background = Join-Path $socialMediaSourceDirectory 'background.png'
        Logo = Join-Path $socialMediaSourceDirectory 'logo.png'
        PunchLine = Join-Path $socialMediaSourceDirectory 'punch-line.png'
        Title = Join-Path $socialMediaSourceDirectory 'title.png'
        Url = Join-Path $socialMediaSourceDirectory 'url.png'
    }

    $canvasDimensions = (& magick identify -format '%w %h' $layerSources.Background) -split ' '
    if ($LASTEXITCODE -ne 0 -or $canvasDimensions.Count -ne 2) {
        throw 'ImageMagick could not determine the social-media canvas dimensions.'
    }

    $canvasWidth = [int] $canvasDimensions[0]
    $canvasHeight = [int] $canvasDimensions[1]
    foreach ($source in $layerSources.Values) {
        $dimensions = (& magick identify -format '%w %h' $source) -split ' '
        if ($LASTEXITCODE -ne 0 -or
            $dimensions.Count -ne 2 -or
            [int] $dimensions[0] -ne $canvasWidth -or
            [int] $dimensions[1] -ne $canvasHeight) {
            throw "Social-media layer '$source' must be ${canvasWidth}x${canvasHeight}."
        }
    }

    $logoGeometry = & magick identify -format '%@' $layerSources.Logo
    if ($LASTEXITCODE -ne 0 -or $logoGeometry -notmatch '^(?<Width>\d+)x(?<Height>\d+)\+(?<X>\d+)\+(?<Y>\d+)$') {
        throw 'ImageMagick could not determine the social-media logo placement.'
    }

    $logoWidth = [int] $Matches.Width
    $logoHeight = [int] $Matches.Height
    $logoX = [int] $Matches.X
    $logoY = [int] $Matches.Y

    $background = Join-Path $temporaryPath 'social-background.png'
    $punchLine = Join-Path $temporaryPath 'social-punch-line.png'
    $title = Join-Path $temporaryPath 'social-title.png'
    $url = Join-Path $temporaryPath 'social-url.png'
    $fittedLogo = Join-Path $temporaryPath 'social-logo-fitted.png'
    $placedLogo = Join-Path $temporaryPath 'social-logo-placed.png'

    New-ColoredLayer -Source $layerSources.Background -Color $pageBackgroundColor -Destination $background
    New-ColoredLayer -Source $layerSources.PunchLine -Color $bodyTextColor -Destination $punchLine
    New-ColoredLayer -Source $layerSources.Title -Color $hourglassColor -Destination $title
    New-ColoredLayer -Source $layerSources.Url -Color $secondaryTealColor -Destination $url

    Invoke-ImageMagick @(
        $IconComposition,
        '-trim', '+repage',
        '-filter', 'Lanczos',
        '-resize', "${logoWidth}x${logoHeight}",
        '-background', 'none',
        '-gravity', 'center',
        '-extent', "${logoWidth}x${logoHeight}",
        $fittedLogo
    )
    Invoke-ImageMagick @(
        '-size', "${canvasWidth}x${canvasHeight}",
        'canvas:none',
        $fittedLogo,
        '-geometry', "+${logoX}+${logoY}",
        '-composite',
        $placedLogo
    )

    $destination = Join-Path $sourceDirectory 'chrononuensis-social-media.png'
    Invoke-ImageMagick @(
        $background,
        $placedLogo,
        $punchLine,
        $title,
        $url,
        '-background', 'none',
        '-layers', 'flatten',
        '-alpha', 'off',
        '-strip',
        '-define', 'png:exclude-chunk=date,time',
        $destination
    )
}

New-Item -ItemType Directory -Path $temporaryPath | Out-Null

try {
    $iconComposition = Join-Path $temporaryPath 'icon-composition.png'
    New-IconComposition -Destination $iconComposition

    if ($Target -in 'All', 'Icon') {
        New-IconAssets -Composition $iconComposition
    }

    if ($Target -in 'All', 'SocialMedia') {
        New-SocialMediaBanner -IconComposition $iconComposition
    }
}
finally {
    $resolvedTemporaryPath = [System.IO.Path]::GetFullPath($temporaryPath)
    $resolvedSourceDirectory = [System.IO.Path]::GetFullPath($sourceDirectory) + [System.IO.Path]::DirectorySeparatorChar

    if ($resolvedTemporaryPath.StartsWith($resolvedSourceDirectory, [System.StringComparison]::OrdinalIgnoreCase)) {
        Remove-Item -LiteralPath $resolvedTemporaryPath -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Output $sourceDirectory
