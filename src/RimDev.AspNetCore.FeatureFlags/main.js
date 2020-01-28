const featuresContainer = document.querySelector('#features');
const notificationsContainer = document.querySelector('#notifications-container');

const fetchOptions = {
  credentials: 'same-origin'
};

fetch('/_features/get_all', fetchOptions)
  .then(res => res.json())
  .then(json => {
    const features = json.map(feature => `<li class="mdl-list__item mdl-list__item--three-line">
      <span class="mdl-list__item-primary-content">
        <i class="material-icons mdl-list__item-avatar">outlined_flag</i>
        <span>${feature.name}</span>
        ${feature.description ? '<span class="mdl-list__item-text-body">' + feature.description + '</span>' : ''}
      </span>
      <span class="mdl-list__item-secondary-content">
        <a class="mdl-list__item-secondary-action" href="#">
          <label class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="${feature.name}">
            <input type="checkbox" id="${feature.name}" class="mdl-switch__input"${feature.value ? " checked" : ""} />
          </label>
        </a>
      </span>
    </li>`);

    featuresContainer.innerHTML = features.join('');

    [...featuresContainer.getElementsByClassName('mdl-switch')].forEach(toggle => {
      componentHandler.upgradeElement(toggle);
    });

    document.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
      checkbox.addEventListener('change', evt => {
        const feature = evt.currentTarget.id;
        const checked = evt.currentTarget.checked;

        fetch('/_features/set', {
          method: 'POST',
          body: JSON.stringify({
            feature: feature,
            value: checked
          }),
          headers: { 'Content-Type': 'application/json' },
          ...fetchOptions
        }).then(() => {
          notificationsContainer.MaterialSnackbar.showSnackbar({
            message: `${feature} set to ${checked}`
          });
        }).catch(err => {
          const toggle = document.getElementById(feature).parentElement;

          if (checked) {
            toggle.MaterialSwitch.off();
          } else {
            toggle.MaterialSwitch.on();
          }

          notificationsContainer.MaterialSnackbar.showSnackbar({
            message: `ERROR: ${err}`
          });
        });
      });
    });
  })
  .catch(err => {
    notificationsContainer.MaterialSnackbar.showSnackbar({
      message: `ERROR: ${err}`
    });
  });
