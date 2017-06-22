/**
 * System configuration for Angular 2 samples
 * Adjust as necessary for your application needs.
 */
(function (global) {
  System.config({
    paths: {
      // paths serve as alias
      'npm:': '/node_modules/'
    },
    // map tells the System loader where to look for things
    map: {
      // our app is within the app folder
      app: '/app',

      // angular bundles
      '@angular/core': 'npm:@angular/core/bundles/core.umd.js',
      '@angular/common': 'npm:@angular/common/bundles/common.umd.js',
      '@angular/compiler': 'npm:@angular/compiler/bundles/compiler.umd.js',
      '@angular/platform-browser': 'npm:@angular/platform-browser/bundles/platform-browser.umd.js',
      '@angular/platform-browser-dynamic': 'npm:@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js',
      '@angular/http': 'npm:@angular/http/bundles/http.umd.js',
      '@angular/router': 'npm:@angular/router/bundles/router.umd.js',
      '@angular/forms': 'npm:@angular/forms/bundles/forms.umd.js',
      'moment': '/node_modules/moment',
        
      //right click module
      'ng2-right-click-menu':'npm:ng2-right-click-menu/src/sh-context-menu.module.js',
      //drag drop bundle 
      'angular-draggable-droppable': 'npm:angular-draggable-droppable/dist/umd/angular-draggable-droppable.js',
      //resizable bundle
      'angular2-resizable': 'npm:angular2-resizable/dist/umd/angular2-resizable.js',

       //ng2-slider-componen
      'ng2-slider-component': '/node_modules/ng2-slider-component',
      'ng2-slideable-directive': '/node_modules/ng2-slideable-directive',
      'ng2-styled-directive': '/node_modules/ng2-styled-directive',

       //nouislider
      'nouislider': '/node_modules/nouislider',
      'ng2-nouislider': '/node_modules/ng2-nouislider',
      // other libraries
      'rxjs': 'npm:rxjs'
    },
    // packages tells the System loader how to load when no filename and/or no extension
    packages: {
      'app': {
        main: './main.js',
        defaultExtension: 'js'
      },
      'appevents': {
          main: './mainevents.js',
          defaultExtension: 'js'
      },
      'rxjs': {
        defaultExtension: 'js'
      },
      'moment': {
          main: '/node_modules/moment/moment.js',
          tyoe: 'cjs',
          defaultExtension: 'js'
      },
    'angular2-in-memory-web-api': { main: 'index.js', defaultExtension: 'js' },
    'angular2-modal': { defaultExtension: 'js', main: 'bundles/angular2-modal.umd' },
    'ng2-slider-component': { main: 'ng2-slider.component.system.js', defaultExtension: 'system.js'  },
    'ng2-slideable-directive':    { defaultExtension: 'js'  },
    'ng2-styled-directive': { defaultExtension: 'js' },
    'nouislider': { main: 'distribute/nouislider.js', defaultExtension: 'js' },
    'ng2-nouislider': { main: 'src/nouislider.js', defaultExtension: 'js' },
    }
  });
})(this);