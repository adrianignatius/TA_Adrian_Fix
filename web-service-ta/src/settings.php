<?php
return [
    'settings' => [
        'displayErrorDetails' => true, // set to false in production
        'addContentLengthHeader' => false, // Allow the web server to send the content-length header
        'upload_directory' => __DIR__ . '/../public/uploads', // upload directory
        // Renderer settings
        'renderer' => [
            'template_path' => __DIR__ . '/../templates/',
        ],

        // Monolog settings
        'logger' => [
            'name' => 'slim-app',
            'path' => isset($_ENV['docker']) ? 'php://stdout' : __DIR__ . '/../logs/app.log',
            'level' => \Monolog\Logger::DEBUG,
        ],

        // Database Settings
        'db' => [
            'host' => 'ta-istts.com',
            'user' => 'u648955938_ta_adrian',
            'pass' => '=6WN4dCUw',
            'dbname' => 'u648955938_ta_adrian',
            'driver' => 'mysql'
        ]
    ],
];
